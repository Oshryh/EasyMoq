namespace EasyMoq.Tests.TestModelClasses
{
    public class Class3 : Interface3
    {
        public virtual string Method1()
        {
            return $"{nameof(Class3)}.{nameof(Method1)}";
        }

        public virtual string Method2()
        {
            return $"{nameof(Class3)}.{nameof(Method2)}";
        }

        public virtual string Method3CallingMethod1()
        {
            return Method1();
        }

    }
}
