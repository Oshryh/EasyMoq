namespace EasyMoq.Tests.TestModelClasses
{
    public interface IInterface2
    {
        string Method1_UsingClass1Method1();
        string Method2_UsingClass1Method3CallingMethod1();
        string Method3_UsingClass3Method1();
        string Method4_UsingClass3Method3CallingMethod1();
    }
}
