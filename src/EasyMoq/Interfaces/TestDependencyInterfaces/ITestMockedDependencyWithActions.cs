using System;
using Moq;

namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestMockedDependencyWithActions<TMockedDependency> : ITestMockedDependencyAction
        where TMockedDependency : class
    {
        ITestMockedDependencyWithActions<TMockedDependency> AndAction(Action<Mock<TMockedDependency>> mockAction);
    }
}