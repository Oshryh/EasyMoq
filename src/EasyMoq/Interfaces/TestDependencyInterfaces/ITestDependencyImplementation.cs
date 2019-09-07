using System;

namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestDependencyImplementation
    {
        Type GetDependencyInterface();
        Type GetDependencyClass();
    }
}