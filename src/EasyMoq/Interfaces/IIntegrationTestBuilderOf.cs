using EasyMoq.Interfaces.TestDependencyInterfaces;

namespace EasyMoq.Interfaces
{
    public interface IIntegrationTestBuilderOf<TTest>
        where TTest : class
    {
        IIntegrationTestBuilderOf<TTest> WithTestDependenciesToMock(params ITestDependencyImplementation[] testDependencies);
        IIntegrationTestBuilderOf<TTest> WithTestMockActions(params ITestMockedDependencyAction[] mockActions);
        IIntegrationTestBuilderOf<TTest> WithTestStaticDependenciesToMock(params ITestStaticDependency[] staticTestDependencies);
    }

    public interface IIntegrationTestBuilderOf<TITest, TTest>
        where TITest : class
        where TTest : class, TITest
    {
        IIntegrationTestBuilderOf<TITest, TTest> WithTestDependenciesToMock(params ITestDependencyImplementation[] testDependencies);
        IIntegrationTestBuilderOf<TTest> WithTestMockActions(params ITestMockedDependencyAction[] mockActions);
        IIntegrationTestBuilderOf<TITest, TTest> WithTestStaticDependenciesToMock(params ITestStaticDependency[] staticTestDependencies);
    }
}