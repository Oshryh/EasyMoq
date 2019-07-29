using EasyMoq.Tests.TestModelClasses;
using EasyMoq.Tests.TestModelClasses.ForStaticClassesTests;
using FluentAssertions;
using Moq;
using Xunit;

namespace EasyMoq.Tests
{
    public class TestedMockOfStaticFunctionality : BaseServiceTest<IInterface1, Class1>
    {
        [Fact]
        public void TestedService_TestMockedClassMethodCalling()
        {
            const string expectedResult = "MockTest";

            TestConfiguration.AddTypeToBeMockedAsStatic(typeof(IDatabaseSettings));

            var mockedDatabaseSettings = GetRelatedMock<IDatabaseSettings>();
            mockedDatabaseSettings.Setup(x => x.GetSettingsValue("Key1")).Returns(()=>expectedResult);
            ConfigurationManager.WithDatabaseSettings(mockedDatabaseSettings.Object);

            var result = GetTestedService().Method4CallingStaticConfigKey1();

            result.Should().Be(expectedResult);
        }


        [Fact]
        public void TestedService_TestMockedClassMethodCallingUnMockedMethod_ShouldBeNull()
        {
            TestConfiguration.AddTypeToBeMockedAsStatic(typeof(IDatabaseSettings));
            ConfigurationManager.WithDatabaseSettings(GetRelatedMock<IDatabaseSettings>().Object);

            var result = GetTestedService().Method5CallingStaticConfigKey2();

            result.Should().BeNull();
        }


    }
}
