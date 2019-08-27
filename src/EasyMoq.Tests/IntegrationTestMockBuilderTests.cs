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
            var container = new WindsorContainer().Install(new Installer());

            var integrationTestMockBuilder = new IntegrationTestMockBuilder<ITestIntegrationApp, TestIntegrationApp>(container,
                typeof(IExternalSupplierClass));

            var mockDataFromSupplier = "Mock data from supplier";

            integrationTestMockBuilder.GetRelatedMock<IExternalSupplierClass>()
                .Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier);

            var expectedResult = "Logged a message" + mockDataFromSupplier + "Data from DB" + "Other data from DB";

            var result = integrationTestMockBuilder.GetTestedService().DoStuffWithServices();
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Test2()
        {
            var container = new WindsorContainer().Install(new Installer());

            var integrationTestMockBuilder = new IntegrationTestMockBuilder<ITestIntegrationApp, TestIntegrationApp>(container,
                typeof(IExternalSupplierClass), typeof(ILoggerClass));
            integrationTestMockBuilder.TestConfiguration.CoupleInterfaceWithClass<ILoggerClass, LoggerClass>();

            var mockDataFromSupplier = "Mock data from supplier";

            integrationTestMockBuilder.GetRelatedMock<IExternalSupplierClass>()
                .Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier);

            var expectedResult = "Logged a message" + mockDataFromSupplier + "Data from DB" + "Other data from DB";

            var result = integrationTestMockBuilder.GetTestedService().DoStuffWithServices();
            result.Should().Be(expectedResult);
        }
    }
}
