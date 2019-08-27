using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.Windsor;
using Moq;
using Component = Castle.MicroKernel.Registration.Component;

namespace EasyMoq
{
    /// <summary>
    /// Provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The service being test.</typeparam>
    /// <typeparam name="TIService">The interface of the service which methods are tested.</typeparam>
    public class MockBuilder<TIService, TService> : IDisposable, IMockBuilder<TIService> 
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
            BuildAsync().GetAwaiter().GetResult();
        }

        public TIService GetTestedService()
        {
            return GetTestedServiceAsync().GetAwaiter().GetResult();
        }

        public Mock<TIService> GetTestedMockService()
        {
            return GetTestedMockServiceAsync().GetAwaiter().GetResult();
        }

        public Mock<T> GetRelatedMock<T>()
            where T : class
        {
            return GetRelatedMockAsync<T>().GetAwaiter().GetResult();
        }

        [Obsolete("No longer in use")]
        public T Resolve<T>()
        {
            return ResolveAsync<T>().GetAwaiter().GetResult(); 
        }

        public void RegisterServiceInstance<TInstance>(TInstance instance)
            where TInstance : class
        {
            RegisterServiceInstanceAsync(instance).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _mockBuilderContainer.Dispose();
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

        private async Task AddStaticOfMockToContainerAsync(Type type)
        {
            await Task.Delay(10).ConfigureAwait(false);
            _mockBuilderContainer.Register(Component.For(typeof(Mock<>).MakeGenericType(type))
                .Instance(_typeMocker.GetInstanceOfMockOfStaticOf(type)));
        }

        public async Task BuildAsync()
        {
            await Task.Delay(10).ConfigureAwait(false);

            if (!TestConfiguration.IsConfigurationRebuildRequired()) return;

            if (_built)
            {
                _mockBuilderContainer.Dispose();
                InitializeNewMockBuilder();
            }

            ApplyDefaultClassesForInterfacesFromAssemblies();

            var parametersTypesToRegister = _typeHelpers.GetTypesDependencies(typeof(TService));
            parametersTypesToRegister.Add(typeof(TIService));

            await Task.WhenAll(TestConfiguration.GetTypesToBeMockedAsStatic().Select(AddStaticOfMockToContainerAsync));
            _typeMocker.RegisterTypes(parametersTypesToRegister, TestConfiguration.GetImplementationTypes());

            TestConfiguration.SetConfigurationBuilt();
            _built = true;
        }

        public async Task<Mock<T>> GetRelatedMockAsync<T>() where T : class
        {
            await BuildAsync().ConfigureAwait(false);
            var mock = _mockBuilderContainer.Resolve<Mock<T>>();
            return mock;
        }

        public async Task<Mock<TIService>> GetTestedMockServiceAsync()
        {
            return await GetRelatedMockAsync<TIService>().ConfigureAwait(false); ;
        }

        public async Task<TIService> GetTestedServiceAsync()
        {
            var service = await GetTestedMockServiceAsync().ConfigureAwait(false); ;
            return service.Object;
        }

        public async Task RegisterServiceInstanceAsync<TInstance>(TInstance instance) where TInstance : class
        {
            await BuildAsync().ConfigureAwait(false);
            _mockBuilderContainer.Register(Component.For<TInstance>().Instance(instance));
        }

        [Obsolete("No longer in use")]
        public async Task<T> ResolveAsync<T>()
        {
            await BuildAsync().ConfigureAwait(false);
            return _mockBuilderContainer.Resolve<T>();
        }
    }
}
