using EasyMoq.Tests.ListOfDependenciesBug.TestClasses;
using FluentAssertions;
using Xunit;

namespace EasyMoq.Tests.ListOfDependenciesBug
{
    public class RelatedMockTests
    {

        [Fact]
        public void TestedServiceWthListOfDependencies()
        {
            const string testValue = "Mock taxi transportation";

            var mockBuilder = MockBuilder.IntegrationTest<ITransportationService, TransportationService>(
                new Installer(),

                TestDependency
                    .Of<ITransportationProvider>()
                    .ImplementedBy<TaxiProvider>()
                    .WithAction(mock => mock
                        .Setup(x => x.GetFormOfTransportation()).Returns(testValue))
            );

            var result = mockBuilder.GetTestedService().GetTransportation(TransportationType.Taxi);

            result.Should().Be(testValue);
        }

    }
}
