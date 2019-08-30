using Moq;
using System;
using System.Threading.Tasks;

namespace EasyMoq
{
    public interface IMockBuilder<TService> : IDisposable
        where TService : class
    {
        TestConfiguration TestConfiguration { get; }

        void Build();
        Mock<T> GetRelatedMock<T>() where T : class;
        Mock<TService> GetTestedMockService();
        TService GetTestedService();
        void RegisterServiceInstance<TInstance>(TInstance instance) where TInstance : class;
        T Resolve<T>();

        Task BuildAsync();
        Task<Mock<T>> GetRelatedMockAsync<T>() where T : class;
        Task<Mock<TService>> GetTestedMockServiceAsync();
        Task<TService> GetTestedServiceAsync();
        Task RegisterServiceInstanceAsync<TInstance>(TInstance instance) where TInstance : class;
        Task<T> ResolveAsync<T>();
    }

    public interface IMockBuilder<TIService, TService> : IMockBuilder<TService>
        where TIService : class
        where TService : class, TIService
    {
        new Mock<TIService> GetTestedMockService();
        new TIService GetTestedService();
        new Task<Mock<TIService>> GetTestedMockServiceAsync();
        new Task<TIService> GetTestedServiceAsync();

    }
}