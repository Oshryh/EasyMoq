using EasyMoq.Tests.TestModelClasses;
using EasyMoq.Tests.TestModelClasses.ForStaticClassesTests;
using FluentAssertions;
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

            AddMockActionOf<IDatabaseSettings>(iDbSettingsMock =>
            {
                iDbSettingsMock.Setup(x => x.GetSettingsValue("Key1")).Returns(() => expectedResult);
                ConfigurationManager.WithDatabaseSettings(iDbSettingsMock.Object);
            });

            var result = GetTestedService().Method4_CallingStaticConfigKey1();

            result.Should().Be(expectedResult);
        }


        [Fact]
        public void TestedService_TestMockedClassMethodCallingUnMockedMethod_ShouldBeNull()
        {
            TestConfiguration.AddTypeToBeMockedAsStatic(typeof(IDatabaseSettings));

            AddMockActionOf<IDatabaseSettings>(iDbSettingsMock => ConfigurationManager.WithDatabaseSettings(iDbSettingsMock.Object));

            var result = GetTestedService().Method5_CallingStaticConfigKey2();

            result.Should().BeNull();
        }


    }
}
