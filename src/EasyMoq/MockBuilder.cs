using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Moq;
using Component = Castle.MicroKernel.Registration.Component;

namespace EasyMoq
{
    public class MockBuilder<TIService, TService> : IDisposable
        where TIService : class
        where TService : class, TIService
    {
        public TestConfiguration TestConfiguration { get; } = new TestConfiguration();
        private AutoMoqResolver _autoMoqSubResolver;

        private IWindsorContainer _mockBuilderContainer;
        private bool _built;
        private TypeHelpers _typeHelpers;
        private TypeMocker _typeMocker;

        public MockBuilder()
        {
            InitializeNewMockBuilder();
        }

        public MockBuilder(bool build) : this()
        {
            if (build)
                Build();
        }

        public void Build(bool forceRebuild = false)
        {
            if (_built && !forceRebuild) return;

            if (_built && forceRebuild)
            {
                _mockBuilderContainer.Dispose();
                InitializeNewMockBuilder();
            }

            ApplyTestConfiguration();

            var parametersTypesToRegister = _typeHelpers.GetTypesDependencies(typeof(TService));

            _autoMoqSubResolver.AddRegisteredTypeRange(parametersTypesToRegister);
            _typeMocker.RegisterTypes(_mockBuilderContainer, parametersTypesToRegister, TestConfiguration.GetImplementationTypes());

            foreach (var type in TestConfiguration.GetTypesToBeMockedAsStatic())
                AddStaticOfMockToContainer(type);

            _mockBuilderContainer.Register(
                Component.For<TIService>()
                    .Instance((TIService)_typeMocker.RegisterMockAndGetInstance(_mockBuilderContainer, typeof(TService), typeof(TIService))));

            _built = true;
        }

        private void ApplyTestConfiguration()
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
            _typeMocker = new TypeMocker(_typeHelpers);
            _autoMoqSubResolver = new AutoMoqResolver(_mockBuilderContainer.Kernel);
            _mockBuilderContainer.Kernel.Resolver.AddSubResolver(_autoMoqSubResolver);
        }

        private void RegisterAllDependencies(IList<Type> parametersTypesToRegister, IReadOnlyDictionary<Type, Type> implementationTypes)
        {
            var allDependenciesToRegister = new List<Type>(parametersTypesToRegister);
            if (TestConfiguration.AllRunningRelevantTypes.Any())
            {
                var uncoupledInterfaces =
                    parametersTypesToRegister.Where(p => p.IsInterface && !implementationTypes.ContainsKey(p));

                foreach (var parameterType in uncoupledInterfaces)
                {
                    var inheritingClasses = TestConfiguration.AllRunningRelevantTypes
                        .Where(p => parameterType.IsAssignableFrom(p)).ToList();

                    if (inheritingClasses.Count == 1)
                    {
                        var inheritingClass = inheritingClasses[0];
                        TestConfiguration.CoupleInterfaceWithClass(parameterType, inheritingClass);
                        if (!allDependenciesToRegister.Contains(inheritingClass))
                            allDependenciesToRegister.Add(inheritingClass);
                    }
                }
            }
        }

        private void AddStaticOfMockToContainer(Type type)
        {
            _mockBuilderContainer.Register(Component.For(typeof(Mock<>).MakeGenericType(type))
                .Instance(_typeMocker.GetInstanceOfMockOfStaticOf(type)));

            _autoMoqSubResolver.AddRegisteredType(type);
        }

        public TIService GetTestedService()
        {
            var service = _mockBuilderContainer.Resolve<TIService>();
            return service;
        }

        public Mock<TIService> GetTestedMockService()
        {
            var service = _mockBuilderContainer.Resolve<Mock<TIService>>();
            return service;
        }

        public Mock<T> GetRelatedMock<T>()
            where T : class
        {
            var mock = _mockBuilderContainer.Resolve<Mock<T>>();
            return mock;
        }

        public void ReleaseMock<TInterface>()
            where TInterface : class
        {
            _mockBuilderContainer.Release(GetRelatedMock<TInterface>());
        }

        public void RegisterServiceInstance<TInstance>(TInstance instance)
            where TInstance : class
        {
            _mockBuilderContainer.Register(Component.For<TInstance>().Instance(instance));
        }

        public void Dispose()
        {
            _mockBuilderContainer.Dispose();
        }
    }
}
