using FluentAssertions;
using MockEverything.Moq.XUnit.Tests.TestModelClasses;
using Moq;
using Xunit;

namespace MockEverything.Moq.XUnit.Tests
{
    public class TestedServiceMockingFunctionality : BaseServiceTest<Interface1, Class1>
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
            var testedService = GetTestedService();

            GetTestedMockService().Setup(x => x.Method1()).Returns(testValue);

            var result = testedService.Method1();

            result.Should().Be(testValue);

            // Verify other methods (with no mocked behavior) are still acting the same.
            testedService.Method2().Should().Be($"{nameof(Class1)}.{nameof(testedService.Method2)}");
        }

        [Fact]
        public void TestedService_TestMockedMethodCallingByOtherNonMockedMethod()
        {
            const string testValue = "MockTest";
            var testedService = GetTestedService();

            GetTestedMockService().Setup(x => x.Method1()).Returns(testValue);

            var result = GetTestedMockService().Object.Method3CallingMethod1();

            result.Should().Be(testValue);

            // Verify other methods (with no mocked behavior) are still acting the same.
            testedService.Method2().Should().Be($"{nameof(Class1)}.{nameof(testedService.Method2)}");
        }

    }
}
