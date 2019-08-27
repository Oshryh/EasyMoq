using Moq;
using System;
using System.Threading.Tasks;

namespace EasyMoq
{
    public interface IMockBuilder<TIService> : IDisposable
        where TIService : class
    {
        TestConfiguration TestConfiguration { get; }

        void Build();
        Mock<T> GetRelatedMock<T>() where T : class;
        Mock<TIService> GetTestedMockService();
        TIService GetTestedService();
        void RegisterServiceInstance<TInstance>(TInstance instance) where TInstance : class;
        T Resolve<T>();

        Task BuildAsync();
        Task<Mock<T>> GetRelatedMockAsync<T>() where T : class;
        Task<Mock<TIService>> GetTestedMockServiceAsync();
        Task<TIService> GetTestedServiceAsync();
        Task RegisterServiceInstanceAsync<TInstance>(TInstance instance) where TInstance : class;
        Task<T> ResolveAsync<T>();
    }
}