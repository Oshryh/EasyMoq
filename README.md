# EasyMoq  [![Build status](https://ci.appveyor.com/api/projects/status/km7el3evvelhtl6f/branch/master?svg=true)](https://ci.appveyor.com/project/OshryHorn/easymoq/branch/master) [![NuGet Badge](https://buildstats.info/nuget/EasyMoq)](https://www.nuget.org/packages/EasyMoq/)
This tiny, simple to use, and very configurable tool, helps writing unit-tests and integration tests for well structured code, as well as imperfect code, which is based on IOc (dependencies, classes and interfaces, are passed in the constructors).  
<sub>I'm adding more examples and documentation as time permits. If you have specific requests or questions, please ask! :)<sub>

## What for?
EasyMoq can take a class, mocks all of its dependencies (as taken in the constructor) recursively (will mock as possible and configured the dependencies of the dependencies) and leave us to write only the code that's really relevant to the test.

## How to install?
The easiest way would be to just install the NuGet package through the Visual Studio Package Manager, or to run the following in the Package Manager Console: ```Install-Package EasyMoq```

## How to use? (simple example)
Let's say we want to test the 'GetInfoFromExternalSupplierAndDb()' method of following class:  
<sub>Notice the class has both interfaces and classes as dependencies<sub>
```csharp
public class LibraryClass : ILibraryClass
{
    private readonly ExternalSupplierClass _externalSupplier;
    private readonly ILoggerClass _logger;
    private readonly IMonitorClass _monitor;
    private readonly DataProviderClass _dataProviderClass;
    private readonly IBusinessLogicClass _businessLogic;
    private readonly IFeatureServiceClass _featureService;

    public LibraryClass(IExternalSupplierClass externalSupplier, 
    ILoggerClass logger, 
    IMonitorClass monitor, 
    DataProviderClass dataProviderClass, 
    IBusinessLogicClass businessLogic, 
    IFeatureServiceClass featureService)
    {
        _externalSupplier = externalSupplier;
        _logger = logger;
        _monitor = monitor;
        _dataProviderClass = dataProviderClass;
        _businessLogic = businessLogic;
        _featureService = featureService;
    }

    public string GetInfoFromExternalSupplierAndDb()
    {
        return $"Data from supplier: {_externalSupplier.GetDataFromUnreliableSupplier()}";
    }
    
    // More code in the class...
}
```
With EasyMoq all we have to do is this:
```csharp
[Fact]
public void WithEasyMoqTest()
{
    var mockBuilder = new MockBuilder<ILibraryClass, LibraryClass>();

    var mockDataFromSupplier = "Mocked data from test supplier";
    var expectedResult = $"Data from supplier: {mockDataFromSupplier}";

    mockBuilder.GetRelatedMock<IExternalSupplierClass>()
        .Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier);

    var result = mockBuilder.GetTestedService().GetInfoFromExternalSupplierAndDb();
    result.Should().Be(expectedResult);
}
```
Basically, EasyMoq did all the rest for us. At least 10 lines of pointless code no longer needed, everything is dynamic and immune to irrelevant changes, and so nice and readable! ^_^

### Another example:
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

    public interface ILoggerClass
    {
        string LogMessage();
    }

    public class LoggerClass : ILoggerClass
    {
        public string LogMessage()
        {
            return "Logged a message";
        }
    }

    public interface ILibraryClass
    {
        string GetInfoFromExternalSupplierAndDb();
    }

    public class LibraryClass : ILibraryClass
    {
        private readonly IExternalSupplierClass _externalSupplier;
        private readonly ILoggerClass _logger;

        public LibraryClass(IExternalSupplierClass externalSupplier, ILoggerClass logger)
        {
            _externalSupplier = externalSupplier;
            _logger = logger;
        }

        public string GetInfoFromExternalSupplierAndDb()
        {
            return $"Data from supplier: {_externalSupplier.GetDataFromUnreliableSupplier()}";
        }

        public string MethodWeAreNotTestingRightNow()
        {
            return $"Logger message: {_logger.LogMessage()}";
        }
    }
```
Then normally, in order to test LibraryClass, we would normally do the following:
```csharp
[Fact]
public void OldWayTest()
{
    var mockDataFromSupplier = "Mocked data from test supplier";
    var expectedResult = $"Data from supplier: {mockDataFromSupplier}";

    var loggerMock = new Mock<ILoggerClass>();
    var externalSupplierClassMock = new Mock<IExternalSupplierClass>();
    externalSupplierClassMock.Setup(x => x.GetDataFromUnreliableSupplier())
        .Returns(() => mockDataFromSupplier);

    var testedService = new LibraryClass(externalSupplierClassMock.Object, loggerMock.Object);

    var result = testedService.GetInfoFromExternalSupplierAndDb();
    result.Should().Be(expectedResult);
}
```
#### But instead, with this framework it will be one of following two options:
1.
```csharp
[Fact]
public void WithEasyMoqTest()
{
    var mockBuilder = new MockBuilder<ILibraryClass, LibraryClass>();

    var mockDataFromSupplier = "Mocked data from test supplier";
    var expectedResult = $"Data from supplier: {mockDataFromSupplier}";

    mockBuilder.GetRelatedMock<IExternalSupplierClass>()
        .Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier);

    var result = mockBuilder.GetTestedService().GetInfoFromExternalSupplierAndDb();
    result.Should().Be(expectedResult);
}
```
2.
```csharp
public class LibraryClassWithBaseTests : BaseServiceTest<ILibraryClass, LibraryClass>
{
    [Fact]
    public void WithEasyMoqTest()
    {
        var mockDataFromSupplier = "Mocked data from test supplier";
        var expectedResult = $"Data from supplier: {mockDataFromSupplier}";

        GetRelatedMock<IExternalSupplierClass>()
            .Setup(x => x.GetDataFromUnreliableSupplier()).Returns(() => mockDataFromSupplier);

        var result = GetTestedService().GetInfoFromExternalSupplierAndDb();
        result.Should().Be(expectedResult);
    }
}
```
* Notice the improvement in readability!
* Notice the reduction in the number of lines. We went down from 9 lines to 6/7 lines, and this is just with 2 dependencies!

## Advantages/features:
- Saves many lines of code. ([See a Simple EasyMoq Example](https://github.com/Oshryh/EasyMoq/wiki/Simple-Mock-2))
- Makes tests more flexible and durable (since there's no need to fix all the tests when a something like ILogger or IMonitor is added to some constructor) and makes life easier.
- Enables you to test imperfect code by defaultively leaving the original functionality accessible (examples will be added, for now see tests)
- Can couple an interface with a class, and by that enable the use any of the class's original functionality through the tested class without mocking or creating anything. Any functionality which is mocked will still use the mock and not the base. (examples will be added, for now see tests)
- Provides a solution for mocking static dependencies with minimal code change. (will add an example of how to implement soon)
- 🆕 Added the IntegrationTestMockBuilder class which takes a container, and specific classes/interfaces to mock, and mocks only requested classes/interfaces, while using the normal behaviour of rest of the dependencies. This is intended for usage in integration tests, where we sometimes want to mock just one method which get changing data from the DB or calls an unreliable third party, but still test the rest of the process. ([See an example here](https://github.com/Oshryh/EasyMoq/wiki/Integration-Test-Mock-1))
