using System;
using System.Collections.Generic;

namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestMockedDependencyAction
    {
        IEnumerable<Action<MockBuilder>> GetMockedDependencyActions();
    }

    public interface ITestStaticDependency
    {
        Type GetStaticDependencyType();
    }

    public interface ITestDependencyImplementation
    {
        Type GetDependencyType();
        Type GetDependencyChildType();
    }

    public interface ITestDependency : ITestStaticDependency, ITestMockedDependencyAction, ITestDependencyImplementation
    {
    }

    public interface ITestDependency<TInterface> :
        ITestDependencyImplementer<TInterface>,
        ITestMockedDependency<TInterface>
        where TInterface : class
    {
    }
}