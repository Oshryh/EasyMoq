using EasyMoq.Tests.TestModelClasses;
using FluentAssertions;
using Xunit;

namespace EasyMoq.Tests
{
    public class RelatedMockTests : BaseServiceTest<Interface2, Class2>
    {
        [Fact]
        public void TestedService_TestOriginalClassMethodCalling()
        {
            var result = GetTestedService().UsingClass1Method1();

            result.Should().Be($"{nameof(Class1)}.Method1");
        }

        [Fact]
        public void TestedService_TestMockedClassMethodCalling()
        {
            const string testValue = "MockTest";
            var testedService = GetTestedService();

            GetRelatedMock<Class1>().Setup(x => x.Method1()).Returns(testValue);

            var result = testedService.UsingClass1Method1();

            result.Should().Be(testValue);
        }

        [Fact]
        public void TestedService_TestOriginalInterfaceMethodCalling()
        {
            var result = GetTestedService().UsingClass3Method1();

            // When calling an un-mocked method on a mocked interface, it should not run, and return the default (string -> null).
            // This will change when using CoupleInterfaceWithClass() in the Prepare() method. Will be tested separately. 
            result.Should().BeNull();
        }

        [Fact]
        public void TestedService_TestMockedInterfaceMethodCalling()
        {
            const string testValue = "MockTest";
            var testedService = GetTestedService();

            GetRelatedMock<Interface3>().Setup(x => x.Method1()).Returns(testValue);

            var result = testedService.UsingClass3Method1();

            result.Should().Be(testValue);
        }

    }
}
