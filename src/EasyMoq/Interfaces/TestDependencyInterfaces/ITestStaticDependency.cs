namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestStaticDependency<TDependencyType> : ITestDependencyImplementer<TDependencyType>
        where TDependencyType : class
    {
    }
}