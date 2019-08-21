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
            using (var container = new WindsorContainer())
            {
                container.Install(new Installer());
                using (var integrationTestMockBuilder = new IntegrationTestMockBuilder<ITestIntegrationApp, TestIntegrationApp>(container,
                    typeof(IExternalSupplierClass), typeof(ILoggerClass)))
                {
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
    }
}
