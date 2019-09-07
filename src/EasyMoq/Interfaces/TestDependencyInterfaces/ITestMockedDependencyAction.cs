using System;
using System.Collections.Generic;

namespace EasyMoq.Interfaces.TestDependencyInterfaces
{
    public interface ITestMockedDependencyAction
    {
        List<Action<MockBuilder>> GetMockedDependencyActions();
    }
}