using System;
using System.Collections.Generic;
using System.Text;

namespace MockEverything.Moq.XUnit.Tests.TestModelClasses
{
    public class Class2 : Interface2
    {
        private readonly Class1 _class1;
        private readonly Interface3 _class3;

        public Class2(Class1 class1, Interface3 class3)
        {
            _class1 = class1;
            _class3 = class3;
        }

        public string UsingClass1Method1()
        {
            return _class1.Method1();
        }

        public string UsingClass1Method3CallingMethod1()
        {
            return _class1.Method3CallingMethod1();
        }

        public string UsingClass3Method1()
        {
            return _class3.Method1();
        }

        public string UsingClass3Method3CallingMethod1()
        {
            return _class3.Method3CallingMethod1();
        }
    }
}
