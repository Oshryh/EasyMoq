namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestDependencyImplementer<in TInterface> where TInterface : class
    {
        ITestDependencyImplementation ImplementedBy<TDependencyClass>()
            where TDependencyClass : class, TInterface;
    }
}