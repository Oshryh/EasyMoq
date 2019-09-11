using System;
using System.Collections.Generic;

namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestMockedDependencyAction
    {
        List<Action<MockBuilder>> GetMockedDependencyActions();
    }

    public interface ITestStaticDependency
    {
        Type GetStaticDependencyType();
    }

    public interface ITestDependencyImplementation
    {
        Type GetDependencyChildType();
    }

    public interface ITestDependency : ITestStaticDependency, ITestMockedDependencyAction, ITestDependencyImplementation
    {
        bool IsStatic { get; }
        Type GetDependencyType();
    }

    public interface ITestDependency<TInterface> :
        ITestDependencyImplementer<TInterface>,
        ITestMockedDependency<TInterface>
        where TInterface : class
    {
    }
}