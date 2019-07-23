using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasyMoq
{
    public class TestConfiguration
    {
        private readonly Dictionary<Type, Type> _implementationTypes = new Dictionary<Type, Type>();
        public List<string> AssembliesNamesParts { get; } = new List<string>();
        public bool UseDefaultClassesForInterfacesFromAssemblies { get; set; }

        public List<Type> AllRunningRelevantTypes { get; private set; }

        public void CoupleInterfaceWithClass<TInterface, TClass>()
        {
            _implementationTypes.Add(typeof(TInterface), typeof(TClass));
        }

        public void CoupleInterfaceWithClass(Type interfaceKey, Type classValue)
        {
            _implementationTypes.Add(interfaceKey, classValue);
        }

        public void AddAssemblyNamePartFilter(string assemblyNamePartFilter)
        {
            AssembliesNamesParts.Add(assemblyNamePartFilter);
        }
        
        public IReadOnlyDictionary<Type, Type> GetImplementationTypes()
        {
            return new ReadOnlyDictionary<Type, Type>(_implementationTypes);
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
