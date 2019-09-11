using Castle.Windsor;
using FluentAssertions;
using Xunit;

namespace EasyMoq.Examples.Example3_IntegrationTestMock_WithDefaultInstanceBehavior
{
    public class IntegrationTestMockBuilderTests
    {
        [Fact]
        public void Test2()
        {
            var integrationTestMockBuilder = MockBuilder.IntegrationTest<ITestIntegrationApp, TestIntegrationApp>(
                new Installer(),
                config => config.WithTestDependenciesToMock(
                    TestDependency.Of<IExternalSupplierClass>(),
                    TestDependency.Of<ILoggerClass>().ImplementedBy<LoggerClass>())
            );

            var mockDataFromSupplier = "Mock data from supplier";

            integrationTestMockBuilder.AddMockActionOf<IExternalSupplierClass>(mock =>
                mock.Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier));

            var expectedResult = "Logged a message" + mockDataFromSupplier + "Data from DB" + "Other data from DB";

            var result = integrationTestMockBuilder.GetTestedService().DoStuffWithServices();
            result.Should().Be(expectedResult);

            integrationTestMockBuilder.Dispose();
        }
    }
}
