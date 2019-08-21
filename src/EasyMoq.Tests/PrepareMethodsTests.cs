using EasyMoq.Tests.TestModelClasses;
using FluentAssertions;
using Moq;
using Xunit;

namespace EasyMoq.Tests
{
    public class PrepareMethodsTests : BaseServiceTest<IInterface2, Class2>
    {
        [Fact]
        public void CoupleInterfaceWithClass_TestDependentNonCoupledInterfaceUnMockedMethodCalling()
        {
            var result = GetTestedService().Method3_UsingClass3Method1();

            result.Should().BeNull();
        }

        [Fact]
        public void CoupleInterfaceWithClass_TestRebuildCoupledInterfaceUnMockedMethodCalling()
        {
            var result = GetTestedService().Method3_UsingClass3Method1();

            result.Should().BeNull();

            // Configuration change
            TestConfiguration.CoupleInterfaceWithClass<IInterface3, Class3>();
            // Rebuilding the mock objects

            var expectedResult = $"{nameof(Class3)}.Method1";
            result = GetTestedService().Method3_UsingClass3Method1();

            // After coupling the interface Interface3 with the class Class3, when calling an un-mocked method on the mocked
            // interface, it should run implemented method in the coupled inheriting class. 
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void CoupleInterfaceWithClassFromAssemblies_TestRebuildCoupledInterfaceUnMockedMethodCalling()
        {
            var result = GetTestedService().Method3_UsingClass3Method1();
            result.Should().BeNull();

            // Configuration change
            TestConfiguration.AddAssemblyNamePartFilter("EasyMoq");
            TestConfiguration.UseDefaultClassesForInterfacesFromAssemblies = true;
            
            // Rebuilding the mock objects

            var expectedResult = $"{nameof(Class3)}.Method1";
            result = GetTestedService().Method3_UsingClass3Method1();

            // After coupling the interface Interface3 with the class Class3, when calling an un-mocked method on the mocked
            // interface, it should run implemented method in the coupled inheriting class. 
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void CoupleInterfaceWithClass_TestDependentCoupledInterfaceMockedMethodCalling()
        {
            var expectedResult = $"{nameof(Class3)}.MockedMethod";

            GetRelatedMock<IInterface3>().Setup(x => x.Method1()).Returns(expectedResult);
            var result = GetTestedService().Method3_UsingClass3Method1();

            // After coupling the interface Interface3 with the class Class3, when calling an un-mocked method on the mocked
            // interface, it should run implemented method in the coupled inheriting class. 
            result.Should().Be(expectedResult);
        }

    }
}
