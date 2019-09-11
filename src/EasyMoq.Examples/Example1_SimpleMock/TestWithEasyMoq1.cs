using FluentAssertions;
using Xunit;

namespace EasyMoq.Examples.Example1_SimpleMock
{
    public class TestWithEasyMoq1 : BaseServiceTest<IInterface3, Class3>
    {
        [Fact]
        public void Test()
        {
            AddMockActionOf<IInterface1>(i1 => i1.Setup(x => x.Method1()).Returns("+test"));

            var testResult = GetTestedService().UsingClass2Method1();

            testResult.Should().Be("Class3.UsingClass2Method1() = Class2.Method1+test");
        }
    }
}
