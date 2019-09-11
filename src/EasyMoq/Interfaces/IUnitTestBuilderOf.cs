using EasyMoq.Interfaces.TestDependencyInterfaces;

namespace EasyMoq.Interfaces
{
    public interface IUnitTestBuilderOf<TTest>
        where TTest : class
    {
        IUnitTestBuilderOf<TTest> WithTestDependencies(params ITestDependencyImplementation[] testDependencies);
        IUnitTestBuilderOf<TTest> WithTestMockActions(params ITestMockedDependencyAction[] mockActions);
        IUnitTestBuilderOf<TTest> WithTestStaticDependencies(params ITestStaticDependency[] staticTestDependencies);
    }

    public interface IUnitTestBuilderOf<TITest, TTest>
        where TITest : class
        where TTest : class, TITest
    {
        IUnitTestBuilderOf<TITest, TTest> WithTestDependencies(params ITestDependencyImplementation[] testDependencies);
        IUnitTestBuilderOf<TITest, TTest> WithTestMockActions(params ITestMockedDependencyAction[] mockActions);
        IUnitTestBuilderOf<TITest, TTest> WithTestStaticDependencies(params ITestStaticDependency[] staticTestDependencies);
    }
}