using FluentAssertions;
using Xunit;

namespace EasyMoq.Examples.Example1_SimpleMock
{
    public class TestWithEasyMoq2
    {
        [Fact]
        public void Test()
        {
            //var mockBuilder = MockBuilder.TestBuilderOf<LibraryClass>()
            //    .WithTestDependencies(
            //        Dependency<ILoggerClass>.ImplementedBy<LoggerClass>(),
            //        Dependency<IFeatureServiceClass>.ImplementedBy<FeatureServiceClass>())
            //    .WithMocks(
            //        Dependency<IExternalSupplierClass>.Mock(supplier =>
            //                supplier.Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => "Mocked data from test supplier"))
            //            .Build();

            //var mockBuilder1 = MockBuilder.UnitTest<Class3>(config => config
            //    .WithTestDependencies(
            //        TestDependency.Of<IInterface1>().ImplementedBy<Class1>())
            //    .WithTestMockActions(
            //        TestDependency.Of<IInterface1>()
            //            .WithAction(supplier => supplier.Setup(x => x.Method1()).Returns("+test"))
            //            .AndAction(supplier => supplier.Setup(x => x.Method1()).Returns("+test")),
            //        TestDependency.Of<IInterface1>().WithAction(supplier => supplier.Setup(x => x.Method1()).Returns("+test")))
            //    .WithTestStaticDependencies(
            //        TestDependency.OfStatic<IInterface1>())
            //        );

            var mockBuilder = MockBuilder.UnitTest<IInterface3, Class3>();
            mockBuilder.TestConfiguration.CoupleInterfaceWithClass<IInterface1, Class1>();

            mockBuilder.AddMockActionOf<IInterface1>(i1=>i1.Setup(x => x.Method1()).Returns("+test"));

            var testResult = mockBuilder.GetTestedService().UsingClass2Method1();
            testResult.Should().Be("Class3.UsingClass2Method1() = Class2.Method1+test");
        }
    }
}
