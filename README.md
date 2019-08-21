# EasyMoq  [![Build status](https://ci.appveyor.com/api/projects/status/km7el3evvelhtl6f/branch/master?svg=true)](https://ci.appveyor.com/project/OshryHorn/easymoq/branch/master)

#### I'm adding more examples and documentation as time permits. If you have specific requests or questions, please ask! :)

This tiny, simple to use, and very configurable tool, helps writing unit-tests and integration tests for well structured code, as well as imperfect code, which is based on ICO (dependencies are passed in the constructors).

EasyMoq can take a class, mocks all of its dependencies (as taken in the constructor) recursively (will mock as possible and configured the dependencies of the dependencies) and leave us to write only the code that's really relevant to the test.

Advantages/features:
- Saves many lines of code.
- Makes tests more flexible and durable (since there's no need to fix all the tests when a something like ILogger or IMonitor is added to some constructor) and makes life easier.
- Enables you to test imperfect code by defaultively leaving the original functionality accessible (examples will be added, for now see tests)
- Can couple an interface with a class, and by that enable the use any of the class's original functionality through the tested class without mocking or creating anything. Any functionality which is mocked will still use the mock and not the base. (examples will be added, for now see tests)
- Provides a solution for mocking static dependencies with minimal code change. (will add an example of how to implement soon)
- **(NEW!!!)** Added the IntegrationTestMockBuilder class which takes a container, and specific classes/interfaces to mock, and mocks only requested classes/interfaces, while using the normal behaviour of rest of the dependencies. This is intended for usage in integration tests, where we sometimes want to mock just one method which get changing data from the DB or calls an unreliable third party, but still test the rest of the process. *See examples 2 and 3* :)

## Examples

#### Example 1
If we have the following classes and interfaces:
```csharp
public interface IInterface1
{
    string Method1();
}

public class Class1 : IInterface1
{
    public virtual string Method1()
    {
        return "Class1.Method1";
    }
}

public class Class2
{
    private readonly IInterface1 _class1;

    public Class2(IInterface1 class1)
    {
        _class1 = class1;
    }

    public virtual string Method1UsingClass1Method1()
    {
        return "Class2.Method1" + _class1.Method1();
    }
}

public interface IInterface3
{
    string UsingClass2Method1();
}
```

And we use them in a third class as follows:
```csharp
public class Class3 : IInterface3
{
    private readonly Class2 _class2;

    public Class3(Class2 class2)
    {
        _class2 = class2;
    }

    public string UsingClass2Method1()
    {
        return "Class3.UsingClass2Method1() = " + _class2.Method1UsingClass1Method1();
    }
}
```
Then normally, in order to test Class3, we would normally do the following:
```csharp
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
```

**But instead, with this framework it will be 1 of two options:**

1)
```csharp
public class TestWithEasyMoq1 : BaseServiceTest<IInterface3, Class3>
{
    [Fact]
    public void Test()
    {
        GetRelatedMock<IInterface1>().Setup(x => x.Method1()).Returns("+test");

        var testResult = GetTestedService().UsingClass2Method1();
        testResult.Should().Be("Class3.UsingClass2Method1() = Class2.Method1+test");
    }
}
```
2)
```csharp
public class TestWithEasyMoq2
{
    [Fact]
    public void Test()
    {
        var mockBuilder = new MockBuilder<IInterface3, Class3>(true);
        mockBuilder.GetRelatedMock<IInterface1>().Setup(x => x.Method1()).Returns("+test");

        var testResult = mockBuilder.GetTestedService().UsingClass2Method1();
        testResult.Should().Be("Class3.UsingClass2Method1() = Class2.Method1+test");
    }
}
```

So, in this example we had only 1 class/interface in each constructor, but many times we have 10 or 20, especially when using IOC, and this tool saves a lot of code, which later also needs to be maintained.



#### Example 2 - IntegrationTestMock - One method mocked and one running normally in the same class
If we have the following classes and interfaces:
```csharp
    public interface IExternalSupplierClass
    {
        string GetDataFromUnreliableSupplier();
    }

    class ExternalSupplierClass : IExternalSupplierClass
    {
        public string GetDataFromUnreliableSupplier()
        {
            return "Get data from unreliable supplier.";
        }
    }

    public class DbProviderClass
    {
        public string GetOtherDataFromDb()
        {
            return "Other data from DB";
        }

        public string GetDataFromDb()
        {
            return "Data from DB";
        }
    }

    public interface IInternalLogicClass
    {
        string GetInfoFromExternalSupplierAndDb();
    }

    class InternalLogicClass : IInternalLogicClass
    {
        private readonly IExternalSupplierClass _externalSupplier;
        private readonly DbProviderClass _dbProvider;

        public InternalLogicClass(IExternalSupplierClass externalSupplier, DbProviderClass dbProvider)
        {
            _externalSupplier = externalSupplier;
            _dbProvider = dbProvider;
        }

        public string GetInfoFromExternalSupplierAndDb()
        {
            return _externalSupplier.GetDataFromUnreliableSupplier() + _dbProvider.GetDataFromDb();
        }
    }

    public interface ITestIntegrationApp
    {
        string DoStuffWithServices();
    }

    public class TestIntegrationApp : ITestIntegrationApp
    {
        private readonly DbProviderClass _dbProvider;
        private readonly IInternalLogicClass _internalLogic;

        public TestIntegrationApp(DbProviderClass dbProvider, IInternalLogicClass internalLogic)
        {
            _dbProvider = dbProvider;
            _internalLogic = internalLogic;
        }

        public string DoStuffWithServices()
        {
            return _internalLogic.GetInfoFromExternalSupplierAndDb()
                   + _dbProvider.GetOtherDataFromDb();
        }
    }
```
And we have the following installer:
```csharp
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EasyMoq.Examples.Example2_IntegrationTestMock_OneMethodMockedAnd1RunningNormally
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<DbProviderClass>(),
                Component.For<IExternalSupplierClass>().ImplementedBy<ExternalSupplierClass>(),
                Component.For<IInternalLogicClass>().ImplementedBy<InternalLogicClass>(),
                Component.For<ITestIntegrationApp>().ImplementedBy<TestIntegrationApp>()
            );
        }
    }
} 

```
We can do the following, and it works perfectly:
```csharp
using Castle.Windsor;
using FluentAssertions;
using Xunit;

namespace EasyMoq.Examples.Example2_IntegrationTestMock_OneMethodMockedAnd1RunningNormally
{
    public class IntegrationTestMockBuilderTests
    {
        [Fact]
        public void Test1()
        {
            var container = new WindsorContainer().Install(new Installer());

            var integrationTestMockBuilder = new IntegrationTestMockBuilder<ITestIntegrationApp, TestIntegrationApp>(container,
                typeof(IExternalSupplierClass));

            var mockDataFromSupplier = "Mock data from supplier";

            integrationTestMockBuilder.GetRelatedMock<IExternalSupplierClass>()
                .Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier);

            var expectedResult = mockDataFromSupplier + "Data from DB" + "Other data from DB";

            var result = integrationTestMockBuilder.GetTestedService().DoStuffWithServices();
            result.Should().Be(expectedResult);
        }
    }
}

```

WOW!!! AMAZING :P

But seriously, this is a really nice addition, and for me it made integration tests so much easier to write (with removing changing/unreliable factors) ... I recommend it whole heartedly! ^_^

