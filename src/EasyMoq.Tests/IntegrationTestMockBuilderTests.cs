using Castle.Windsor;
using EasyMoq.Tests.TestModelClasses.ForIntegrationTestMockBuilderTests;
using FluentAssertions;
using Xunit;

namespace EasyMoq.Tests
{
    public class IntegrationTestMockBuilderTests
    {
        [Fact]
        public void Test1()
        {
            var integrationTestMockBuilder = MockBuilder.IntegrationTest<ITestIntegrationApp, TestIntegrationApp>(
                new Installer(),
                config => config.WithTestDependenciesToMock(TestDependency.Of<IExternalSupplierClass>()));

            var mockDataFromSupplier = "Mock data from supplier";

            integrationTestMockBuilder.AddMockActionOf<IExternalSupplierClass>(mock =>
                mock.Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier));

            var expectedResult = "Logged a message" + mockDataFromSupplier + "Data from DB" + "Other data from DB";

            var result = integrationTestMockBuilder.GetTestedService().DoStuffWithServices();
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Test2()
        {
            var integrationTestMockBuilder = MockBuilder.IntegrationTest<ITestIntegrationApp, TestIntegrationApp>(
                new Installer(),
                config => config.WithTestDependenciesToMock(
                    TestDependency.Of<IExternalSupplierClass>(),
                    TestDependency.Of<ILoggerClass>().ImplementedBy<LoggerClass>()
                ));

            var mockDataFromSupplier = "Mock data from supplier";

            integrationTestMockBuilder.AddMockActionOf<IExternalSupplierClass>(mock =>
                mock.Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier));

            var expectedResult = "Logged a message" + mockDataFromSupplier + "Data from DB" + "Other data from DB";

            var result = integrationTestMockBuilder.GetTestedService().DoStuffWithServices();
            result.Should().Be(expectedResult);
        }
    }
}
