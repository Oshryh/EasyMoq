using System;
using System.Collections.Generic;

namespace EasyMoq.Interfaces
{
    public interface ITestConfiguration
    {
        List<Type> AllRunningRelevantTypes { get; }
        MockStrategy MockStrategy { get; }
        bool UseDefaultClassesForInterfacesFromAssemblies { get; set; }

        void AddAssemblyNamePartFilter(string assemblyNamePartFilter);
        void AddMockToRun(Action<MockBuilder> mockAction);
        void AddTypesToMock(IEnumerable<Type> typesToMock);
        void AddTypeToBeMockedAsStatic(Type typeToMockAsStatic);
        void AddTypeToMock(Type typeToMock);
        void AddTypeToMock<T>();
        void CoupleInterfaceWithClass(Type interfaceKey, Type classValue);
        void CoupleInterfaceWithClass<TInterface, TClass>();
        IReadOnlyDictionary<Type, Type> GetImplementationTypes();
        IReadOnlyList<Type> GetTypesToBeMockedAsStatic();
        IReadOnlyList<Type> GetTypesToMock();
        bool IsTypeToMock(Type typesToMock);
        bool IsTypeToMock<T>();
        bool TryGetImplementationType(Type parameterType, out Type inheritingClass);
        void SetConfigurationBuilt();
        void SetRunningRelevantTypes(List<Type> allRunningRelevantTypes);
        bool IsConfigurationRebuildRequired();
        IReadOnlyList<Action<MockBuilder>> GetMockActions();
        IReadOnlyList<string> GetAssembliesWildcards();

    }
}