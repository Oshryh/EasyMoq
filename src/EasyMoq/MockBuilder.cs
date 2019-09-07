using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EasyMoq.Interfaces;
using EasyMoq.Interfaces.TestDependencyInterfaces;
using Moq;
using Component = Castle.MicroKernel.Registration.Component;

namespace EasyMoq
{
    public abstract partial class MockBuilder
    {
        #region Static Methods

        public static IMockBuilder<TService> UnitTest<TService>()
            where TService : class
        {
            return new MockBuilder<TService>();
        }

        public static IMockBuilder<TIService, TService> UnitTest<TIService, TService>()
            where TIService : class
            where TService : class, TIService
        {
            return new MockBuilder<TIService, TService>();
        }

        public static IMockBuilder<TService> UnitTest<TService>(Action<IUnitTestBuilderOf<TService>> func)
            where TService : class
        {
            var testBuilder = new MockBuilder<TService>();
            func.Invoke(testBuilder);

            return testBuilder;
        }

        public static IMockBuilder<TIService, TService> UnitTest<TIService, TService>(Action<IUnitTestBuilderOf<TIService, TService>> func)
            where TIService : class
            where TService : class, TIService
        {
            var testBuilder = new MockBuilder<TIService, TService>();
            func.Invoke(testBuilder);

            return testBuilder;
        }

        public static IMockBuilder<TService> IntegrationTest<TService>(IWindsorInstaller windsorInstaller, Action<IIntegrationTestBuilderOf<TService>> func)
            where TService : class
        {
            var testBuilder = new MockBuilder<TService>(windsorInstaller);
            func.Invoke(testBuilder);

            return testBuilder;
        }

        public static IMockBuilder<TIService, TService> IntegrationTest<TIService, TService>(IWindsorInstaller windsorInstaller, Action<IIntegrationTestBuilderOf<TIService, TService>> func)
            where TIService : class
            where TService : class, TIService
        {
            var testBuilder = new MockBuilder<TIService, TService>(windsorInstaller);
            func.Invoke(testBuilder);

            return testBuilder;
        }

        public static IMockBuilder<TService> IntegrationTest<TService>(IWindsorInstaller windsorInstaller)
            where TService : class
        {
            var testBuilder = new MockBuilder<TService>(windsorInstaller);

            return testBuilder;
        }

        public static IMockBuilder<TIService, TService> IntegrationTest<TIService, TService>(IWindsorInstaller windsorInstaller)
            where TIService : class
            where TService : class, TIService
        {
            var testBuilder = new MockBuilder<TIService, TService>(windsorInstaller);

            return testBuilder;
        }
        
        #endregion
    }

    /// <summary>
    /// Provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    public abstract partial class MockBuilder
    {
        private readonly IWindsorInstaller _windsorInstaller;

        private AutoMoqResolver _autoMoqSubResolver;
        private IWindsorContainer _mockBuilderContainer;

        private TypeHelpers _typeHelpers;
        private TypeMocker _typeMocker;

        private bool _built;

        protected MockStrategy MockStrategy { get; }
        protected List<Type> TypesToMock { get; }

        public TestConfiguration TestConfiguration { get; } = new TestConfiguration();

        #region Public Methods

        public void AddMockActionOf<T>(Action<Mock<T>> mockAction) where T : class
        {
            TestConfiguration.AddMockToRun(ma => mockAction(ma.GetRelatedMock<T>()));
        }

        public void Dispose()
        {
            _mockBuilderContainer.Dispose();
        }

        #endregion

        #region Internal Methods

        internal MockBuilder()
        {
            MockStrategy = MockStrategy.UnitTest;
            InitializeNewMockBuilder();
        }

        internal MockBuilder(IWindsorInstaller windsorInstaller)
        {
            _windsorInstaller = windsorInstaller;
            MockStrategy = MockStrategy.Integration;
            TypesToMock = new List<Type>();
            InitializeNewMockBuilder();
        }

        #endregion

        #region Protected Methods

        protected Mock<T> GetRelatedMock<T>() where T : class
        {
            if (MockStrategy == MockStrategy.Integration && !TypesToMock.Contains(typeof(T)))
                throw new Exception(
                    $"The type {typeof(T).Name} was not set as a type to be mocked.");

            var mock = _mockBuilderContainer.Resolve<Mock<T>>();
            return mock;
        }

        protected void AddTestDependencies(params ITestDependencyImplementation[] testDependencies)
        {
            foreach (var testDependency in testDependencies)
            {
                TestConfiguration.CoupleInterfaceWithClass(
                    testDependency.GetDependencyInterface(),
                    testDependency.GetDependencyClass());
            }
        }

        protected void AddTestDependenciesToMock(params ITestDependencyImplementation[] testDependencies)
        {
            AddTestDependencies(testDependencies);
            TypesToMock.AddRange(testDependencies.Select(p=>p.GetDependencyInterface()));
        }

        protected void AddTestMockActions(params ITestMockedDependencyAction[] mockActions)
        {
            foreach (var mockAction in mockActions.SelectMany(p => p.GetMockedDependencyActions()))
                TestConfiguration.AddMockToRun(mockAction);
        }

        protected void AddTestStaticDependencies(params ITestStaticDependency[] staticTestDependencies)
        {
            foreach (var staticTestDependency in staticTestDependencies)
                TestConfiguration.AddTypeToBeMockedAsStatic(staticTestDependency.GetStaticDependencyType());
        }

        protected async Task BuildAsync<TService>() where TService : class
        {
            await BuildAsync<TService>(typeof(TService)).ConfigureAwait(false);
        }

        protected async Task BuildAsync<TService>(Type testedTypeToRegister)
        {
            await Task.Delay(10).ConfigureAwait(false);

            if (!TestConfiguration.IsConfigurationRebuildRequired()) return;

            if (_built)
            {
                _mockBuilderContainer.Dispose();
                InitializeNewMockBuilder();
            }

            var typesToRegister = new List<Type>();

            if (MockStrategy == MockStrategy.Integration)
            {
                TypesToMock.AddRange(TestConfiguration.GetTypesToBeMockedAsStatic());
                _autoMoqSubResolver.AddRegisteredTypeRange(TypesToMock);

                typesToRegister = TypesToMock;
            }

            if (MockStrategy == MockStrategy.UnitTest)
            {
                ApplyDefaultClassesForInterfacesFromAssemblies();
                typesToRegister = _typeHelpers.GetTypesDependencies(typeof(TService));
                typesToRegister.Add(testedTypeToRegister);
            }

            await Task.WhenAll(TestConfiguration.GetTypesToBeMockedAsStatic()
                    .Select(p => Task.Run(() => AddStaticOfMockToContainer(p))))
                .ConfigureAwait(false);
            _typeMocker.RegisterTypes(typesToRegister, TestConfiguration.GetImplementationTypes());

            await Task.WhenAll(TestConfiguration.GetMockActions().Select(p => Task.Run(() => p(this))).ToArray())
                .ConfigureAwait(false);

            TestConfiguration.SetConfigurationBuilt();
            _built = true;
        }

        #endregion

        #region Private Methods

        private void InitializeNewMockBuilder()
        {
            _mockBuilderContainer = new WindsorContainer();

            if (MockStrategy == MockStrategy.Integration)
            {
                _mockBuilderContainer.Install(_windsorInstaller);

                _autoMoqSubResolver = new AutoMoqResolver(_mockBuilderContainer.Kernel);
                _mockBuilderContainer.Kernel.Resolver.AddSubResolver(_autoMoqSubResolver);
            }

            _typeHelpers = new TypeHelpers(TestConfiguration);
            _typeMocker = new TypeMocker(_mockBuilderContainer, _typeHelpers, MockStrategy);
        }

        private void ApplyDefaultClassesForInterfacesFromAssemblies()
        {
            if (TestConfiguration.UseDefaultClassesForInterfacesFromAssemblies
                && TestConfiguration.AssembliesNamesParts.Any())
            {
                var allRunningRelevantTypes =
                    _typeHelpers.GetAllTypesFromAssemblies(TestConfiguration.AssembliesNamesParts);
                TestConfiguration.SetRunningRelevantTypes(allRunningRelevantTypes);
            }
        }

        private void AddStaticOfMockToContainer(Type type)
        {
            _mockBuilderContainer.Register(Component.For(typeof(Mock<>).MakeGenericType(type))
                .Instance(_typeMocker.GetInstanceOfMockOfStaticOf(type)));
        }

        #endregion

    }

    /// <summary>
    /// Provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The service being test.</typeparam>
    public class MockBuilder<TService> : MockBuilder, IMockBuilder<TService>, 
        IUnitTestBuilderOf<TService>, IIntegrationTestBuilderOf<TService>
        where TService : class
    {
        public MockBuilder()
        {
        }

        internal MockBuilder(IWindsorInstaller windsorInstaller) : base(windsorInstaller)
        {
        }

        protected virtual async Task BuildAsync()
        {
            await BuildAsync<TService>().ConfigureAwait(false); ;
        }

        public virtual void AddTestedServiceMockAction(Action<Mock<TService>> mockAction)
        {
            AddMockActionOf(mockAction);
        }

        public TService GetTestedService()
        {
            return GetTestedServiceAsync().GetAwaiter().GetResult();
        }

        public virtual async Task<TService> GetTestedServiceAsync()
        {
            await BuildAsync().ConfigureAwait(false); ;
            var service = GetRelatedMock<TService>();
            return service.Object;
        }

        #region IUnitTestBuilderOf<TService> Implementation

        IUnitTestBuilderOf<TService> IUnitTestBuilderOf<TService>.WithTestDependencies(params ITestDependencyImplementation[] testDependencies)
        {
            AddTestDependencies(testDependencies);
            return this;
        }

        IUnitTestBuilderOf<TService> IUnitTestBuilderOf<TService>.WithTestMockActions(params ITestMockedDependencyAction[] mockActions)
        {
            AddTestMockActions(mockActions);
            return this;
        }

        IUnitTestBuilderOf<TService> IUnitTestBuilderOf<TService>.WithTestStaticDependencies(params ITestStaticDependency[] staticTestDependencies)
        {
            AddTestStaticDependencies(staticTestDependencies);
            return this;
        }

        #endregion

        #region IIntegrationTestBuilderOf<TService> Implementation

        IIntegrationTestBuilderOf<TService> IIntegrationTestBuilderOf<TService>.WithTestDependenciesToMock(params ITestDependencyImplementation[] testDependencies)
        {
            AddTestDependencies(testDependencies);
            return this;
        }

        IIntegrationTestBuilderOf<TService> IIntegrationTestBuilderOf<TService>.WithTestMockActions(params ITestMockedDependencyAction[] mockActions)
        {
            AddTestMockActions(mockActions);
            return this;
        }

        IIntegrationTestBuilderOf<TService> IIntegrationTestBuilderOf<TService>.WithTestStaticDependenciesToMock(params ITestStaticDependency[] staticTestDependencies)
        {
            AddTestStaticDependencies(staticTestDependencies);
            return this;
        }

        #endregion

    }

    /// <summary>
    /// Provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The service being test.</typeparam>
    /// <typeparam name="TIService">The interface of the service which methods are tested.</typeparam>
    public class MockBuilder<TIService, TService> : MockBuilder<TService>, IMockBuilder<TIService, TService>,
        IUnitTestBuilderOf<TIService, TService>, IIntegrationTestBuilderOf<TIService, TService>
        where TIService : class
        where TService : class, TIService
    {
        public MockBuilder()
        {
            TestConfiguration.CoupleInterfaceWithClass<TIService, TService>();
            if (MockStrategy == MockStrategy.Integration)
                TypesToMock.Add(typeof(TIService));
        }

        internal MockBuilder(IWindsorInstaller windsorInstaller) : base(windsorInstaller)
        {
            TestConfiguration.CoupleInterfaceWithClass<TIService, TService>();
            if (MockStrategy == MockStrategy.Integration)
                TypesToMock.Add(typeof(TIService));
        }

        private new async Task BuildAsync()
        {
            await BuildAsync<TService>(typeof(TIService)).ConfigureAwait(false);
        }

        public new TIService GetTestedService()
        {
            return GetTestedServiceAsync().GetAwaiter().GetResult();
        }

        public new async Task<TIService> GetTestedServiceAsync()
        {
            await BuildAsync().ConfigureAwait(false);
            var service = GetRelatedMock<TIService>();
            return service.Object;
        }

        public void AddTestedServiceMockAction(Action<Mock<TIService>> mockAction)
        {
            AddMockActionOf(mockAction);
        }

        #region IUnitTestBuilderOf<TIService, TService> Implementation

        IUnitTestBuilderOf<TIService, TService> IUnitTestBuilderOf<TIService, TService>.WithTestDependencies(params ITestDependencyImplementation[] testDependencies)
        {
            AddTestDependencies(testDependencies);
            return this;
        }

        IUnitTestBuilderOf<TIService, TService> IUnitTestBuilderOf<TIService, TService>.WithTestMockActions(params ITestMockedDependencyAction[] mockActions)
        {
            AddTestMockActions(mockActions);
            return this;
        }

        IUnitTestBuilderOf<TIService, TService> IUnitTestBuilderOf<TIService, TService>.WithTestStaticDependencies(params ITestStaticDependency[] staticTestDependencies)
        {
            AddTestStaticDependencies(staticTestDependencies);
            return this;
        }

        #endregion

        #region IIntegrationTestBuilderOf<TIService, TService> Implementation

        IIntegrationTestBuilderOf<TIService, TService> IIntegrationTestBuilderOf<TIService, TService>.WithTestDependenciesToMock(params ITestDependencyImplementation[] testDependencies)
        {
            AddTestDependenciesToMock(testDependencies);
            return this;
        }

        IIntegrationTestBuilderOf<TIService, TService> IIntegrationTestBuilderOf<TIService, TService>.WithTestStaticDependenciesToMock(params ITestStaticDependency[] staticTestDependencies)
        {
            AddTestStaticDependencies(staticTestDependencies);
            return this;
        }

        IIntegrationTestBuilderOf<TService> IIntegrationTestBuilderOf<TIService, TService>.WithTestMockActions(params ITestMockedDependencyAction[] mockActions)
        {
            AddTestMockActions(mockActions);
            return this;
        }

        #endregion

    }

}
