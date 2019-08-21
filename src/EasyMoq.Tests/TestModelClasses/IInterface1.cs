namespace EasyMoq.Tests.TestModelClasses
{
    public interface IInterface1
    {
        string Method1();
        string Method2();
        string Method3_CallingMethod1();
        string Method4_CallingStaticConfigKey1();
        string Method5_CallingStaticConfigKey2();
    }
}
