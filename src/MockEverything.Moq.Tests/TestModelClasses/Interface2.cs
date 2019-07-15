using System;
using System.Collections.Generic;
using System.Text;

namespace MockEverything.Moq.XUnit.Tests.TestModelClasses
{
    public interface Interface2
    {
        string UsingClass1Method1();
        string UsingClass1Method3CallingMethod1();
        string UsingClass3Method1();
        string UsingClass3Method3CallingMethod1();
    }
}
