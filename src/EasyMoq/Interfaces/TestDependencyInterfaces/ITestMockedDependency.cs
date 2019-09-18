using System;
using Moq;
using Moq.Language;

namespace EasyMoq.Interfaces.TestDependencyInterfaces
{

    public interface ITestMockedDependencyWithActionSetup<TMockedDependency, TResult> : IReturns<TMockedDependency, TResult>
        where TMockedDependency : class
    {
    }

    public interface ITestMockedDependency<TMockedDependency> : ITestDependency
        where TMockedDependency : class
    {
        ITestMockedDependencyWithActions<TMockedDependency> WithAction(Action<Mock<TMockedDependency>> mockAction);
    }

    public interface ITestMockedDependencyWithActions<TMockedDependency> : ITestDependency
        where TMockedDependency : class
    {
        ITestMockedDependencyWithActions<TMockedDependency> AndAction(Action<Mock<TMockedDependency>> mockAction);
    }
}