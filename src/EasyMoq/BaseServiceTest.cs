using System;
using System.Threading.Tasks;
using EasyMoq.Interfaces;
using Moq;

namespace EasyMoq
{
    public abstract class BaseServiceTest : IBaseServiceTest, IDisposable
    {
        private readonly IMockBuilder _mockBuilder;

        protected T GetMockBuilder<T>() where T : IMockBuilder
        {
            return (T)_mockBuilder;
        }

        public ITestConfiguration TestConfiguration => _mockBuilder.TestConfiguration;

        protected BaseServiceTest(IMockBuilder mockBuilder)
        {
            _mockBuilder = mockBuilder;
        }

        public void AddMockActionOf<T>(Action<Mock<T>> mockAction) where T : class
        {
            _mockBuilder.AddMockActionOf(mockAction);
        }

        public void Dispose()
        {
            _mockBuilder.Dispose();
        }

    }

    /// <summary>
    /// Base class which provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The service being test.</typeparam>
    public abstract class BaseServiceTest<TService> : BaseServiceTest, IBaseServiceTest<TService>
        where TService : class
    {
        private IMockBuilder<TService> MockBuilder => GetMockBuilder<IMockBuilder<TService>>();

        protected BaseServiceTest() : base(new MockBuilder<TService>()) { }

        protected BaseServiceTest(MockBuilder<TService> mockBuilder) : base(mockBuilder)
        { }

        public TService GetTestedService()
        {
            return MockBuilder.GetTestedService();
        }

        public virtual void AddTestedServiceMockAction(Action<Mock<TService>> mockAction)
        {
            MockBuilder.AddTestedServiceMockAction(mockAction);
        }

        public async Task<TService> GetTestedServiceAsync()
        {
            return await MockBuilder.GetTestedServiceAsync().ConfigureAwait(false);
        }

    }

    /// <summary>
    /// Base class which provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The service being test.</typeparam>
    /// <typeparam name="TIService">The interface of the service which methods are tested.</typeparam>
    public abstract class BaseServiceTest<TIService, TService> : BaseServiceTest<TService>, IBaseServiceTest<TIService, TService>
        where TIService : class
        where TService : class, TIService
    {
        private IMockBuilder<TIService, TService> MockBuilder => GetMockBuilder<IMockBuilder<TIService, TService>>();

        protected BaseServiceTest() : base(new MockBuilder<TIService, TService>())
        { }

        public void AddTestedServiceMockAction(Action<Mock<TIService>> mockAction)
        {
            MockBuilder.AddTestedServiceMockAction(mockAction);
        }

        public new TIService GetTestedService()
        {
            return MockBuilder.GetTestedService();
        }

        public new async Task<TIService> GetTestedServiceAsync()
        {
            return await MockBuilder.GetTestedServiceAsync().ConfigureAwait(false);
        }

    }
}