using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyMoq
{
    public class TypeHelpers
    {
        private readonly TestConfiguration _testConfiguration;

        public TypeHelpers(TestConfiguration testConfiguration)
        {
            _testConfiguration = testConfiguration;
        }

        internal List<Type> GetAllTypesFromAssemblies(List<string> assembliesNamesParts)
        {
            var runningAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(p => assembliesNamesParts.Any(a => p.FullName.Contains(a)))
                .ToList();
            var allAssemblies = runningAssemblies
                .SelectMany(assembly => assembly.GetReferencedAssemblies()
                    .Where(p => assembliesNamesParts.Any(a => p.FullName.Contains(a)))
                    .Select(Assembly.Load))
                .Union(runningAssemblies)
                .ToList();

            var allRunningRelevantTypes = allAssemblies
                .Where(p => assembliesNamesParts.Any(a => p.FullName.Contains(a)))
                .SelectMany(s => s.GetTypes())
                .Where(type => !type.IsInterface)
                .ToList();

            return allRunningRelevantTypes;
        }

        public bool TryGetSingleImplementingClassType(Type interfaceType, out Type singleInheritingClassType)
        {
            var inheritingClassTypes = _testConfiguration.AllRunningRelevantTypes
                .Where(interfaceType.IsAssignableFrom).ToList();

            singleInheritingClassType = inheritingClassTypes.Count == 1
                ? inheritingClassTypes.SingleOrDefault()
                : null;

            return singleInheritingClassType == null;
        }

        public bool HasEmptyConstructor(Type parameterType)
        {
            return parameterType.GetConstructors().Any(c => c.GetParameters().Length == 0);
        }

        public List<Type> GetConstructorRefParameters(ConstructorInfo constructor)
        {
            return constructor.GetParameters()
                .Where(p => !p.IsOptional)
                .Select(p => p.ParameterType)
                .Where(p => !p.IsValueType && p != typeof(string))
                .ToList();
        }

        public List<Type> GetTypesDependencies(Type type)
        {
            var dependentTypes = new List<Type>();
            var typesToCheck = new Stack<Type>();
            typesToCheck.Push(type);

            while (typesToCheck.Any())
            {
                var currentType = typesToCheck.Pop();

                var constructorsParametersTypes = GetAllDistinctConstructorsParametersTypes(currentType)
                    .Where(p=>!IsChecked(p, dependentTypes)).ToList();

                dependentTypes.AddRange(constructorsParametersTypes);

                foreach (var parameterType in constructorsParametersTypes)
                {
                    if (!parameterType.IsInterface)
                    {
                        typesToCheck.Push(parameterType);
                    }
                    else if (_testConfiguration.TryGetImplementationType(parameterType, out var inheritingClass)
                             || TryGetSingleInheritingType(parameterType, out inheritingClass))
                    {
                        _testConfiguration.CoupleInterfaceWithClass(parameterType, inheritingClass);
                        typesToCheck.Push(inheritingClass);
                    }
                }

            }

            return dependentTypes;
        }

        public bool TryGetSingleInheritingType(Type parameterType, out Type inheritingClass)
        {
            var inheritingClasses = _testConfiguration.AllRunningRelevantTypes.Where(parameterType.IsAssignableFrom).ToList();

            inheritingClass = inheritingClasses.Count == 1 ? inheritingClasses[0] : null;

            return inheritingClass == null;
        }

        private IEnumerable<Type> GetAllDistinctConstructorsParametersTypes(Type type)
        {
            var constructorsParameters =
                type.GetConstructors()
                    .SelectMany(GetConstructorRefParameters)
                    .Where(p => p != type)
                    .Distinct();

            return constructorsParameters;
        }

        private static bool IsChecked(Type type, ICollection<Type> checkedTypes)
        {
            return checkedTypes.Contains(type);
        }

    }
}
