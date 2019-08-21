using Castle.Windsor;
using Moq;
using System;
using System.Linq;
using Component = Castle.MicroKernel.Registration.Component;

namespace EasyMoq
{
    /// <summary>
    /// Provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The service being test.</typeparam>
    /// <typeparam name="TIService">The interface of the service which methods are tested.</typeparam>
    public class MockBuilder<TIService, TService> : IDisposable
        where TIService : class
        where TService : class, TIService
    {
        public TestConfiguration TestConfiguration { get; } = new TestConfiguration();

        private IWindsorContainer _mockBuilderContainer;
        private bool _built;
        private TypeHelpers _typeHelpers;
        private TypeMocker _typeMocker;

        public MockBuilder()
        {
            InitializeNewMockBuilder();
            TestConfiguration.CoupleInterfaceWithClass<TIService, TService>();
        }

        public void Build()
        {
            if (!TestConfiguration.IsConfigurationRebuildRequired()) return;

            if (_built)
            {
                _mockBuilderContainer.Dispose();
                InitializeNewMockBuilder();
            }

            ApplyDefaultClassesForInterfacesFromAssemblies();

            var parametersTypesToRegister = _typeHelpers.GetTypesDependencies(typeof(TService));
            parametersTypesToRegister.Add(typeof(TIService));

            TestConfiguration.GetTypesToBeMockedAsStatic().ToList().ForEach(AddStaticOfMockToContainer);
            _typeMocker.RegisterTypes(parametersTypesToRegister, TestConfiguration.GetImplementationTypes());

            TestConfiguration.SetConfigurationBuilt();
            _built = true;
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

        private void InitializeNewMockBuilder()
        {
            _mockBuilderContainer = new WindsorContainer();
            _typeHelpers = new TypeHelpers(TestConfiguration);
            _typeMocker = new TypeMocker(_mockBuilderContainer, _typeHelpers, MockStrategy.UnitTest);
        }

        private void AddStaticOfMockToContainer(Type type)
        {
            _mockBuilderContainer.Register(Component.For(typeof(Mock<>).MakeGenericType(type))
                .Instance(_typeMocker.GetInstanceOfMockOfStaticOf(type)));
        }

        public TIService GetTestedService()
        {
            Build();
            var service = GetTestedMockService().Object;
            return service;
        }

        public Mock<TIService> GetTestedMockService()
        {
            Build();
            var service = _mockBuilderContainer.Resolve<Mock<TIService>>();
            return service;
        }

        public Mock<T> GetRelatedMock<T>()
            where T : class
        {
            Build();
            var mock = _mockBuilderContainer.Resolve<Mock<T>>();
            return mock;
        }

        public T Resolve<T>()
        {
            Build();
            return _mockBuilderContainer.Resolve<T>();
        }

        public void RegisterServiceInstance<TInstance>(TInstance instance)
            where TInstance : class
        {
            Build();
            _mockBuilderContainer.Register(Component.For<TInstance>().Instance(instance));
        }

        public void Dispose()
        {
            _mockBuilderContainer.Dispose();
        }

    }
}
