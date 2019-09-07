using System;

namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestStaticDependency
    {
        Type GetStaticDependencyType();
    }

    public interface ITestStaticDependency<in TDependencyType>
        : ITestDependencyImplementer<TDependencyType>, ITestStaticDependency
        where TDependencyType : class
    {
    }
}