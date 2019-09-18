using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EasyMoq.Interfaces;

namespace EasyMoq
{
    public class TestConfiguration : ITestConfiguration
    {

        #region Private Read-Only Members

        private readonly List<Type> _typesToBeMockedAsStatic = new List<Type>();
        private readonly Dictionary<Type, Type> _implementationTypes = new Dictionary<Type, Type>();
        private readonly List<Action<MockBuilder>> _mocksToRun = new List<Action<MockBuilder>>();
        private readonly List<Type> _typesToMock = new List<Type>();
        private readonly List<string> _assembliesWildcards = new List<string>();

        #endregion

        #region Private Members

        private bool _rebuildRequired = true;

        #endregion

        #region Public Properties

        public MockStrategy MockStrategy { get; }

        public bool UseDefaultClassesForInterfacesFromAssemblies { get; set; }

        public List<Type> AllRunningRelevantTypes { get; private set; } = new List<Type>();

        #endregion

        #region Constructors

        public TestConfiguration(MockStrategy mockStrategy)
        {
            MockStrategy = mockStrategy;
        }

        #endregion

        #region Private Methods

        private void SetConfigurationRebuildRequired()
        {
            _rebuildRequired = true;
        }

        #endregion

        #region Public Methods

        public bool IsTypeToMock(Type typesToMock)
        {
            return _typesToMock.Contains(typesToMock);
        }

        public bool IsTypeToMock<T>()
        {
            return _typesToMock.Contains(typeof(T));
        }

        public void AddTypeToMock(Type typeToMock)
        {
            if (!_typesToMock.Contains(typeToMock))
                _typesToMock.Add(typeToMock);
        }

        public void AddTypeToMock<T>()
        {
            IsTypeToMock(typeof(T));
        }

        public void AddTypesToMock(IEnumerable<Type> typesToMock)
        {
            typesToMock.ToList().ForEach(AddTypeToMock);
        }

        public IReadOnlyList<Type> GetTypesToMock()
        {
            return _typesToMock;
        }

        public void AddMockToRun(Action<MockBuilder> mockAction)
        {
            if (_mocksToRun.Any(p => p.Method == mockAction.Method && p.Target == mockAction.Target)) return;

            _mocksToRun.Add(mockAction);
            SetConfigurationRebuildRequired();
        }

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
            _assembliesWildcards.Add(assemblyNamePartFilter);
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

        public void SetRunningRelevantTypes(List<Type> allRunningRelevantTypes)
        {
            AllRunningRelevantTypes = allRunningRelevantTypes;
        }

        public void SetConfigurationBuilt()
        {
            _rebuildRequired = false;
        }

        public bool IsConfigurationRebuildRequired()
        {
            return _rebuildRequired;
        }

        public IReadOnlyList<Action<MockBuilder>> GetMockActions()
        {
            return _mocksToRun;
        }

        public IReadOnlyList<string> GetAssembliesWildcards()
        {
            return _assembliesWildcards;
        }

        #endregion

    }
}
