using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EasyMoq.Interfaces.TestDependencyInterfaces;
using Moq;

namespace EasyMoq
{
    public static class TestDependency
    {
        public static ITestDependency<TDependencyInterface> Of<TDependencyInterface>()
            where TDependencyInterface : class
        {
            return new TestDependency<TDependencyInterface>();
        }

        public static ITestStaticDependency<TStaticDependency> OfStatic<TStaticDependency>()
            where TStaticDependency : class
        {
            return new TestDependency<TStaticDependency>();
        }
    }

    public class TestDependency<TDependencyType> :
        ITestDependency<TDependencyType>,
        ITestStaticDependency<TDependencyType>,
        ITestMockedDependencyWithActions<TDependencyType>,
        ITestDependencyImplementation<TDependencyType>
        where TDependencyType : class
    {
        private readonly List<Action<Mock<TDependencyType>>> _mockActions =
            new List<Action<Mock<TDependencyType>>>();

        private readonly Type _dependencyInterfaceType;
        private Type _dependencyClassType;

        internal TestDependency()
        {
            var dependencyType = typeof(TDependencyType);

            if (dependencyType.IsInterface)
                _dependencyInterfaceType = dependencyType;
            else
                _dependencyClassType = dependencyType;
        }

        public Type GetDependencyType()
        {
            return _dependencyInterfaceType;
        }

        public Type GetDependencyChildType()
        {
            return _dependencyClassType;
        }

        public Type GetStaticDependencyType()
        {
            return GetDependencyType();
        }

        public IEnumerable<Action<MockBuilder>> GetMockedDependencyActions()
        {
            return _mockActions.Select(mockAction =>
                    (Action<MockBuilder>)(mockBuilder => mockBuilder.AddMockActionOf(mockAction)))
                .ToList();
        }

        public ITestDependencyImplementation<TDependencyType> ImplementedBy<TDependencyClass>()
            where TDependencyClass : class, TDependencyType
        {
            _dependencyClassType = typeof(TDependencyClass);
            return this;
        }

        ITestMockedDependencyWithActions<TDependencyType> ITestMockedDependency<TDependencyType>.WithAction(
            Action<Mock<TDependencyType>> mockAction)
        {
            _mockActions.Add(mockAction);
            return this;
        }

        ITestMockedDependencyWithActions<TDependencyType> ITestMockedDependencyWithActions<TDependencyType>.AndAction(
            Action<Mock<TDependencyType>> mockAction)
        {
            _mockActions.Add(mockAction);
            return this;
        }

    }
}