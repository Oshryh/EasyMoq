using EasyMoq.Tests.TestModelClasses.ForStaticClassesTests;

namespace EasyMoq.Tests.TestModelClasses
{
    public class Class1 : IInterface1
    {
        public virtual string Method1()
        {
            return $"{nameof(Class1)}.{nameof(Method1)}";
        }

        public virtual string Method2()
        {
            return $"{nameof(Class1)}.{nameof(Method2)}";
        }

        public virtual string Method3_CallingMethod1()
        {
            return Method1();
        }

        public virtual string Method4_CallingStaticConfigKey1()
        {
            return ConfigurationManager.GetSettingsValue("Key1");
        }

        public virtual string Method5_CallingStaticConfigKey2()
        {
            return ConfigurationManager.AppSettings["Key2"];
        }
    }
}
