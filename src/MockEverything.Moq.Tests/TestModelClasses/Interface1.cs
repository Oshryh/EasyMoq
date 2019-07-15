using System;
using System.Collections.Generic;
using System.Text;

namespace MockEverything.Moq.XUnit.Tests.TestModelClasses
{
    public interface Interface1
    {
        string Method1();
        string Method2();
        string Method3CallingMethod1();
    }
}
