using System;
using System.Collections.Generic;
using System.Linq;
using EasyMoq.Interfaces.TestDependencyInterfaces;
using Moq;

namespace EasyMoq
{
    public static class TestDependency
    {
        public static ITestDependencyInterface<TDependencyInterface> OfInterface<TDependencyInterface>()
            where TDependencyInterface : class
        {
            return new TestDependency<TDependencyInterface>();
        }

        public static ITestMockedDependency<TMockedDependency> OfMock<TMockedDependency>()
            where TMockedDependency : class
        {
            return new TestDependency<TMockedDependency>();
        }

        public static ITestStaticDependency<TStaticDependency> OfStatic<TStaticDependency>()
            where TStaticDependency : class
        {
            return new TestDependency<TStaticDependency>();
        }
    }

    public class TestDependency<TDependencyInterface> : ITestDependencyInterface<TDependencyInterface>,
        ITestStaticDependency<TDependencyInterface>, ITestMockedDependency<TDependencyInterface>,
        ITestMockedDependencyWithActions<TDependencyInterface>
        where TDependencyInterface : class
    {
        private readonly List<Action<Mock<TDependencyInterface>>> _mockActions =
            new List<Action<Mock<TDependencyInterface>>>();

        private readonly Type _dependencyInterfaceType;
        private Type _dependencyClassType;

        public Type GetDependencyInterface()
        {
            return _dependencyInterfaceType;
        }

        public Type GetDependencyClass()
        {
            return _dependencyClassType;
        }

        public Type GetStaticDependencyType()
        {
            return GetDependencyInterface();
        }

        internal TestDependency()
        {
            _dependencyInterfaceType = typeof(TDependencyInterface);
        }

        public ITestDependencyImplementation ImplementedBy<TDependencyClass>()
            where TDependencyClass : class, TDependencyInterface
        {
            _dependencyClassType = typeof(TDependencyClass);
            return this;
        }

        public List<Action<MockBuilder>> GetMockedDependencyActions()
        {
            return _mockActions.Select(mockAction =>
                    (Action<MockBuilder>)(mockBuilder => mockBuilder.AddMockActionOf(mockAction)))
                .ToList();
        }

        public ITestMockedDependencyWithActions<TDependencyInterface> WithAction(
            Action<Mock<TDependencyInterface>> mockAction)
        {
            _mockActions.Add(mockAction);
            return this;
        }

        public ITestMockedDependencyWithActions<TDependencyInterface> AndAction(
            Action<Mock<TDependencyInterface>> mockAction)
        {
            return WithAction(mockAction);
        }

    }

}