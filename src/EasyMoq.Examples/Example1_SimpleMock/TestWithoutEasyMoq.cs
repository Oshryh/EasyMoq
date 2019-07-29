using FluentAssertions;
using Moq;
using Xunit;

namespace EasyMoq.Examples.Example1_SimpleMock
{
    public class TestWithoutEasyMoq
    {
        [Fact]
        public void Test()
        {
            var mockIInterface1 = new Mock<IInterface1>();
            var mockClass2 = new Mock<Class2>(mockIInterface1.Object);
            mockClass2.CallBase = true;

            mockIInterface1.Setup(x => x.Method1()).Returns("+test");

            var testedClass = new Class3(mockClass2.Object);

            var testResult = testedClass.UsingClass2Method1();

            testResult.Should().Be("Class3.UsingClass2Method1() = Class2.Method1+test");
        }
    }
}
