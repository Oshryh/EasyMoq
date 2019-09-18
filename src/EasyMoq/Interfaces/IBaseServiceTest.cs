using System;
using System.Threading.Tasks;
using Moq;

namespace EasyMoq.Interfaces
{

    public interface IBaseServiceTest
    {
        ITestConfiguration TestConfiguration { get; }

        void AddMockActionOf<T>(Action<Mock<T>> mockAction) where T : class;
    }

    /// <summary>
    /// Base class which provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The tested service.</typeparam>
    public interface IBaseServiceTest<TService> : IBaseServiceTest
        where TService : class
    {
        TService GetTestedService();
        Task<TService> GetTestedServiceAsync();

        void AddTestedServiceMockAction(Action<Mock<TService>> mockAction);
    }

    /// <summary>
    /// Base class which provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The tested service.</typeparam>
    /// <typeparam name="TIService">The interface of the service to be tested.</typeparam>
    public interface IBaseServiceTest<TIService, TService> : IBaseServiceTest<TService>
        where TIService : class
        where TService : class, TIService
    {
        new TIService GetTestedService();
        new Task<TIService> GetTestedServiceAsync();

        void AddTestedServiceMockAction(Action<Mock<TIService>> mockAction);
    }

}