using Moq;
using System;

namespace EasyMoq
{
    /// <summary>
    /// Base class which provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The service being test.</typeparam>
    /// <typeparam name="TIService">The interface of the service which methods are tested.</typeparam>
    public abstract class BaseServiceTest<TIService, TService> : IBaseServiceTest<TIService>, IDisposable
        where TIService : class
        where TService : class, TIService
    {
        private readonly MockBuilder<TIService, TService> _mockBuilder = new MockBuilder<TIService, TService>();

        public TestConfiguration TestConfiguration => _mockBuilder.TestConfiguration;

        public TIService GetTestedService()
        {
            return _mockBuilder.GetTestedService();
        }

        public Mock<TIService> GetTestedMockService()
        {
            return _mockBuilder.GetTestedMockService();
        }

        public Mock<T> GetRelatedMock<T>() where T : class
        {
            return _mockBuilder.GetRelatedMock<T>();
        }

        public void ReleaseMock<TInterface>() where TInterface : class
        {
            _mockBuilder.GetRelatedMock<TInterface>();
        }

        public void RegisterServiceInstance<TInstance>(TInstance instance) where TInstance : class
        {
            _mockBuilder.RegisterServiceInstance(instance);
        }

        public void Dispose()
        {
            _mockBuilder.Dispose();
        }

    }
}