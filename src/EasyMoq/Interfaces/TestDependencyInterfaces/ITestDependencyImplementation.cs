namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestDependencyImplementation<TImplementation> : ITestMockedDependency<TImplementation>
        where TImplementation : class
    {
    }
}