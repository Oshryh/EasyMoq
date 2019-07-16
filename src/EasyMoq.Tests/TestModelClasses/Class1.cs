namespace EasyMoq.Tests.TestModelClasses
{
    public class Class1 : Interface1
    {
        public virtual string Method1()
        {
            return $"{nameof(Class1)}.{nameof(Method1)}";
        }

        public virtual string Method2()
        {
            return $"{nameof(Class1)}.{nameof(Method2)}";
        }

        public virtual string Method3CallingMethod1()
        {
            return Method1();
        }

    }
}
