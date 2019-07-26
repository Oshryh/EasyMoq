using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasyMoq
{
    public class TestConfiguration
    {
        private readonly List<Type> _typesToBeMockedAsStatic = new List<Type>();
        private readonly Dictionary<Type, Type> _implementationTypes = new Dictionary<Type, Type>();
        public List<string> AssembliesNamesParts { get; } = new List<string>();
        public bool UseDefaultClassesForInterfacesFromAssemblies { get; set; }

        public List<Type> AllRunningRelevantTypes { get; private set; } = new List<Type>();

        public void CoupleInterfaceWithClass<TInterface, TClass>()
        {
            _implementationTypes.Add(typeof(TInterface), typeof(TClass));
        }

        public void CoupleInterfaceWithClass(Type interfaceKey, Type classValue)
        {
            _implementationTypes.Add(interfaceKey, classValue);
        }

        public void AddTypeToBeMockedAsStatic(Type typeToMockAsStatic)
        {
            _typesToBeMockedAsStatic.Add(typeToMockAsStatic);
        }

        public void AddAssemblyNamePartFilter(string assemblyNamePartFilter)
        {
            AssembliesNamesParts.Add(assemblyNamePartFilter);
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


    }
}
