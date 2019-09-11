using EasyMoq.Tests.TestModelClasses;
using FluentAssertions;
using Xunit;

namespace EasyMoq.Tests
{
    public class TestedServiceMockingFunctionality : BaseServiceTest<IInterface1, Class1>
    {
        [Fact]
        public void TestedService_TestOriginalMethodCalling()
        {
            var result = GetTestedService().Method1();

            result.Should().Be($"{nameof(Class1)}.Method1");
        }

        [Fact]
        public void TestedService_TestMockedMethodCalling()
        {
            const string testValue = "MockTest";

            AddTestedServiceMockAction(mock => mock.Setup(x => x.Method1()).Returns(testValue));

            var result = GetTestedService().Method1();

            result.Should().Be(testValue);

            // Verify other methods (with no mocked behavior) are still acting the same.
            GetTestedService().Method2().Should().Be($"{nameof(Class1)}.{nameof(IInterface1.Method2)}");
        }

        [Fact]
        public void TestedService_TestMockedMethodCallingByOtherNonMockedMethod()
        {
            const string testValue = "MockTest";

            AddTestedServiceMockAction(mock => mock.Setup(x => x.Method1()).Returns(testValue));

            var result = GetTestedService().Method3_CallingMethod1();

            result.Should().Be(testValue);

            // Verify other methods (with no mocked behavior) are still acting the same.
            GetTestedService().Method2().Should().Be($"{nameof(Class1)}.{nameof(IInterface1.Method2)}");
        }

    }
}
