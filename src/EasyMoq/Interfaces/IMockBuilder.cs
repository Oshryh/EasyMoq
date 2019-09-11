using System;
using System.Threading.Tasks;
using Moq;

namespace EasyMoq.Interfaces
{
    public interface IMockBuilder : IDisposable
    {
        TestConfiguration TestConfiguration { get; }

        void AddMockActionOf<T>(Action<Mock<T>> mockAction) where T : class;

    }

    public interface IMockBuilder<TService> : IMockBuilder
        where TService : class
    {
        TService GetTestedService();

        Task<TService> GetTestedServiceAsync();

        void AddTestedServiceMockAction(Action<Mock<TService>> mockAction);
    }

    public interface IMockBuilder<TIService, TService> : IMockBuilder<TService>
        where TIService : class
        where TService : class, TIService
    {
        new TIService GetTestedService();

        new Task<TIService> GetTestedServiceAsync();

        void AddTestedServiceMockAction(Action<Mock<TIService>> mockAction);
    }
}