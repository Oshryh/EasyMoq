using Moq;
using System;

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
    }
}