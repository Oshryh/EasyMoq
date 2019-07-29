namespace EasyMoq.Examples.Example1_SimpleMock
{
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
}
