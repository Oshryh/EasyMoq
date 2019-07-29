# EasyMoq

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

    public virtual string Method1AndClass1Method1()
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
        return "Class3.UsingClass2Method1() = " + _class2.Method1();
    }
}
```
Then normally, in order to test Class3, we would normally do the following:
```csharp
public class Class3Test
{
    public string Test()
    {
      var mockIInterface1 = new Mock<IInterface1>();
      var mockClass2 = new Mock<Class2> (mockIInterface1.Object);
      mockClass2.CallBase = true;
      
      mockIInterface1.Setup(x => x.Method1()).Returns("+test");

      var testedClass = new Class3(mockClass2.Object);

      var testResult = testedClass.UsingClass1Method1() + testedClass.UsingClass1Method1();

      testResult.Should().Be("Class3.UsingClass2Method1() = Class2.Method1+test");
    }
}
```

**But instead, with this framework it will be 1 of two options:**
1)
```csharp
public class Class3Test : BaseServiceTest<IInterface3, Class3>
{
    public string Test()
    {
      GetRelatedMock<IInterface1>().Setup(x => x.Method1()).Returns("+test");
      
      var testResult = GetTestedService().UsingClass1Method1() + testedClass.UsingClass1Method1();
      testResult.Should().Be("Class3.UsingClass2Method1() = Class2.Method1+test");
    }
}
```
2)
```csharp
public class Class3Test
{
    public string Test()
    {
      var mockBuilder = new MockBuilder<IInterface3, Class3>()
      mockBuilder.GetRelatedMock<IInterface1>().Setup(x => x.Method1()).Returns("+test");
      
      var testResult = mockBuilder.GetTestedService().UsingClass1Method1() + testedClass.UsingClass1Method1();
      testResult.Should().Be("Class3.UsingClass2Method1() = Class2.Method1+test");
    }
}
```

So, in this example we had only 1 class/interface in each constructor, but many times we have 10 or 20, especially when using IOC. 
This tiny, simple to use, and very configurable tool, saves many lines of code, make your tests more flexable and durable (cause you don't have to fix all the tests becuase you just added a ILogger or IMonitor to some constructor) amd makes life easier.

## More examples and documentation will come soon :) 
