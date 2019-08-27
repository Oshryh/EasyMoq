using Castle.Windsor;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Component = Castle.MicroKernel.Registration.Component;

namespace EasyMoq
{
    /// <summary>
    /// Provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The service being test.</typeparam>
    /// <typeparam name="TIService">The interface of the service which methods are tested.</typeparam>
    public class IntegrationTestMockBuilder<TIService, TService> : IDisposable
        where TIService : class
        where TService : class, TIService
    {
        public TestConfiguration TestConfiguration { get; } = new TestConfiguration();

        private readonly IWindsorContainer _mockBuilderContainer;
        private readonly TypeMocker _typeMocker;
        private readonly List<Type> _typesToMock;
        private readonly AutoMoqResolver _autoMoqSubResolver;

        public IntegrationTestMockBuilder(IWindsorContainer container, params Type[] typesToMock)
        {
            _mockBuilderContainer = container;
            _typesToMock = typesToMock.ToList();
            _typesToMock.Add(typeof(TIService));

            _autoMoqSubResolver = new AutoMoqResolver(_mockBuilderContainer.Kernel);
            _mockBuilderContainer.Kernel.Resolver.AddSubResolver(_autoMoqSubResolver);

            TestConfiguration.CoupleInterfaceWithClass<TIService, TService>();
            var typeHelpers = new TypeHelpers(TestConfiguration);
            _typeMocker = new TypeMocker(_mockBuilderContainer, typeHelpers, MockStrategy.Integration);
        }

        public void Build()
        {
            if (!TestConfiguration.IsConfigurationRebuildRequired()) return;

            _typesToMock.AddRange(TestConfiguration.GetTypesToBeMockedAsStatic());

            _autoMoqSubResolver.AddRegisteredTypeRange(_typesToMock);

            TestConfiguration.GetTypesToBeMockedAsStatic().ToList().ForEach(AddStaticOfMockToContainer);
            _typeMocker.RegisterTypes(_typesToMock, TestConfiguration.GetImplementationTypes());

            TestConfiguration.SetConfigurationBuilt();
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
            if (!_typesToMock.Contains(typeof(T)))
                throw new Exception(
                    $"The type {typeof(T).Name} was not set as a type to be mocked.");

            Build();
            var mock = _mockBuilderContainer.Resolve<Mock<T>>();
            return mock;
        }

        public void Dispose()
        {
            _mockBuilderContainer.Dispose();
        }

    }
}
