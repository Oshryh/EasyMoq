namespace EasyMoq.Tests.TestModelClasses
{
    public class Class2 : IInterface2
    {
        private readonly Class1 _class1;
        private readonly IInterface3 _class3;

        public Class2(Class1 class1, IInterface3 class3)
        {
            _class1 = class1;
            _class3 = class3;
        }

        public string Method1_UsingClass1Method1()
        {
            return _class1.Method1();
        }

        public string Method2_UsingClass1Method3CallingMethod1()
        {
            return _class1.Method3_CallingMethod1();
        }

        public string Method3_UsingClass3Method1()
        {
            return _class3.Method1();
        }

        public string Method4_UsingClass3Method3CallingMethod1()
        {
            return _class3.Method3_CallingMethod1();
        }
    }
}
