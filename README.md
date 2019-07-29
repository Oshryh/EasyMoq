# EasyMoq

This tiny, simple to use, and very configurable tool, which helps write tests for well structured code, as well as imperfect code.

EasyMoq can take a class, mocks all of its dependencies (as taken in the constructor) recursively (will mock as possible and configured the dependencies of the dependencies) and leave us to write only the code that's really relevant to the test.

Advantages/features:
- Saves many lines of code.
- Makes tests more flexible and durable (since there's no need to fix all the tests when a something like ILogger or IMonitor is added to some constructor) and makes life easier.
- Enables you to test imperfect code by defaultively leaving the original functionality accessible (examples will be added, for now see tests)
- Can couple an inteface with a class, and by that enable the use any of the class's original functionality through the tested class without mocking or creating anything. Any functionality which is mocked will still use the mock and not the base. (examples will be added, for now see tests)

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



## More examples and documentation will come soon :)
