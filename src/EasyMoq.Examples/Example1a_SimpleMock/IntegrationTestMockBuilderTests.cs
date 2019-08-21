using FluentAssertions;
using Moq;
using Xunit;

namespace EasyMoq.Examples.Example1a_SimpleMock
{
    public class LibraryClassTests
    {
        [Fact]
        public void OldWayTest()
        {
            var mockDataFromSupplier = "Mocked data from test supplier";
            var expectedResult = $"Data from supplier: {mockDataFromSupplier}";

            var loggerMock = new Mock<ILoggerClass>();
            var externalSupplierClassMock = new Mock<IExternalSupplierClass>();
            externalSupplierClassMock.Setup(x => x.GetDataFromUnreliableSupplier())
                .Returns(() => mockDataFromSupplier);

            var testedService = new LibraryClass(externalSupplierClassMock.Object, loggerMock.Object);

            var result = testedService.GetInfoFromExternalSupplierAndDb();
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void WithEasyMoqTest()
        {
            var mockBuilder = new MockBuilder<ILibraryClass, LibraryClass>();

            var mockDataFromSupplier = "Mocked data from test supplier";
            var expectedResult = $"Data from supplier: {mockDataFromSupplier}";

            mockBuilder.GetRelatedMock<IExternalSupplierClass>()
                .Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier);

            var result = mockBuilder.GetTestedService().GetInfoFromExternalSupplierAndDb();
            result.Should().Be(expectedResult);
        }
    }

    public class LibraryClassWithBaseTests : BaseServiceTest<ILibraryClass, LibraryClass>
    {
        [Fact]
        public void WithEasyMoqTest()
        {
            var mockDataFromSupplier = "Mocked data from test supplier";
            var expectedResult = $"Data from supplier: {mockDataFromSupplier}";

            GetRelatedMock<IExternalSupplierClass>()
                .Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier);

            var result = GetTestedService().GetInfoFromExternalSupplierAndDb();
            result.Should().Be(expectedResult);
        }
    }
}
