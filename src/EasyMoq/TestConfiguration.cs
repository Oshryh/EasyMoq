using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EasyMoq
{
    public class TestConfiguration
    {
        private readonly List<Type> _typesToBeMockedAsStatic = new List<Type>();
        private readonly Dictionary<Type, Type> _implementationTypes = new Dictionary<Type, Type>();

        private bool _rebuildRequired = true;

        public List<string> AssembliesNamesParts { get; } = new List<string>();
        public bool UseDefaultClassesForInterfacesFromAssemblies { get; set; }

        public List<Type> AllRunningRelevantTypes { get; private set; } = new List<Type>();

        public void CoupleInterfaceWithClass<TInterface, TClass>()
        {
            CoupleInterfaceWithClass(typeof(TInterface), typeof(TClass));
        }

        public void CoupleInterfaceWithClass(Type interfaceKey, Type classValue)
        {
            _implementationTypes.Add(interfaceKey, classValue);
            SetConfigurationRebuildRequired();
        }

        public void AddTypeToBeMockedAsStatic(Type typeToMockAsStatic)
        {
            _typesToBeMockedAsStatic.Add(typeToMockAsStatic);
            SetConfigurationRebuildRequired();
        }

        public void AddAssemblyNamePartFilter(string assemblyNamePartFilter)
        {
            AssembliesNamesParts.Add(assemblyNamePartFilter);
            SetConfigurationRebuildRequired();
        }

        public IReadOnlyDictionary<Type, Type> GetImplementationTypes()
        {
            return new ReadOnlyDictionary<Type, Type>(_implementationTypes);
        }

        public IReadOnlyList<Type> GetTypesToBeMockedAsStatic()
        {
            return new ReadOnlyCollection<Type>(_typesToBeMockedAsStatic);
        }

        public bool TryGetImplementationType(Type parameterType, out Type inheritingClass)
        {
            return _implementationTypes.TryGetValue(parameterType, out inheritingClass);
        }

        internal void SetRunningRelevantTypes(List<Type> allRunningRelevantTypes)
        {
            AllRunningRelevantTypes = allRunningRelevantTypes;
        }

        internal void SetConfigurationBuilt()
        {
            _rebuildRequired = false;
        }

        internal void SetConfigurationRebuildRequired()
        {
            _rebuildRequired = true;
        }

        internal bool IsConfigurationRebuildRequired()
        {
            return _rebuildRequired;
        }
    }
}
