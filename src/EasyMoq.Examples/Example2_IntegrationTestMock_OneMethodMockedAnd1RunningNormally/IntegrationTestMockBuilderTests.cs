using Castle.Windsor;
using FluentAssertions;
using Xunit;

namespace EasyMoq.Examples.Example2_IntegrationTestMock_OneMethodMockedAnd1RunningNormally
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

            var expectedResult = mockDataFromSupplier + "Data from DB" + "Other data from DB";

            var result = integrationTestMockBuilder.GetTestedService().DoStuffWithServices();
            result.Should().Be(expectedResult);
        }
    }
}
