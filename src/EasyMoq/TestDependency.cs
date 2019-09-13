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
            return new TestDependency<TStaticDependency>(true);
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
        private readonly Type _dependencyType;
        private Type _dependencyClassType;

        public bool IsStatic { get; }

        internal TestDependency(bool isStatic = false)
        {
            IsStatic = isStatic;
            _dependencyType = typeof(TDependencyType);
        }

        public Type GetDependencyType()
        {
            return _dependencyType;
        }

        public Type GetDependencyChildType()
        {
            return _dependencyClassType;
        }

        public Type GetStaticDependencyType()
        {
            if (!IsStatic) return null;

            return GetDependencyType();
        }

        public List<Action<MockBuilder>> GetMockedDependencyActions()
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