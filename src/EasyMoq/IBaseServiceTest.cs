using Moq;

namespace EasyMoq
{
    /// <summary>
    /// Base class which provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TIService">The interface of the service which methods are tested.</typeparam>
    public interface IBaseServiceTest<TIService>
        where TIService : class
    {
        void ReBuild();
        TIService GetTestedService();
        Mock<TIService> GetTestedMockService();
        Mock<T> GetRelatedMock<T>() where T : class;
        void ReleaseMock<TInterface>() where TInterface : class;
        void RegisterServiceInstance<TInstance>(TInstance instance) where TInstance : class;

    }
}