namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestDependencyImplementer<TInterface> : ITestDependency where TInterface : class
    {
        ITestDependencyImplementation<TInterface> ImplementedBy<TDependencyClass>()
            where TDependencyClass : class, TInterface;
    }
}