namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestDependencyInterface<in TInterface>
        : ITestDependencyImplementer<TInterface>, ITestDependencyImplementation
        where TInterface : class
    {
    }
}