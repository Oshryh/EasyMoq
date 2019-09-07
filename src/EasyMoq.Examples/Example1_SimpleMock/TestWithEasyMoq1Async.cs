using FluentAssertions;
using Xunit;

namespace EasyMoq.Examples.Example1_SimpleMock
{
    public class TestWithEasyMoq1Async : BaseServiceTest<IInterface3, Class3>
    {
        [Fact]
        public async void Test()
        {
            AddMockActionOf<IInterface1>(interface1Mock => interface1Mock.Setup(x => x.Method1()).Returns("+test"));

            var testResult = (await GetTestedServiceAsync()).UsingClass2Method1();

            testResult.Should().Be("Class3.UsingClass2Method1() = Class2.Method1+test");
        }
    }
}
