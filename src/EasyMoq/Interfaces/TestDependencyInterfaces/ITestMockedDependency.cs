using System;
using Moq;

namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestMockedDependency<TMockedDependency> : ITestMockedDependencyAction
        where TMockedDependency : class
    {
        ITestMockedDependencyWithActions<TMockedDependency> WithAction(Action<Mock<TMockedDependency>> mockAction);
    }
}