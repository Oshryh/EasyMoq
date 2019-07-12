using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoqEverything
{
    public class TypeHelpers
    {
        public List<Type> AllRunningRelevantTypes { get; }
        public List<string> AssembliesNamesParts { get; } = new List<string>{ "CHS" };

        public TypeHelpers()
        {
            var runningAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(p => AssembliesNamesParts.Any(a=>p.FullName.Contains(a)))
                .ToList();
            var allAssemblies = runningAssemblies
                .SelectMany(assembly => assembly.GetReferencedAssemblies()
                    .Where(p => AssembliesNamesParts.Any(a => p.FullName.Contains(a)))
                    .Select(Assembly.Load))
                .Union(runningAssemblies)
                .Distinct()
                .ToList();

            AllRunningRelevantTypes = allAssemblies
                .Where(p => AssembliesNamesParts.Any(a => p.FullName.Contains(a)))
                .SelectMany(s => s.GetTypes())
                .Where(type => !type.IsInterface)
                .ToList();
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

        public List<Type> GetConstructorsParametersTypes(Type type, Dictionary<Type, Type> implementationTypes)
        {
            var constructorsParameters = GetConstructorsParametersTypes(type, implementationTypes, new List<Type>());

            return constructorsParameters;
        }

        private List<Type> GetConstructorsParametersTypes(Type type, Dictionary<Type, Type> implementationTypes, List<Type> checkedTypes)
        {
            var parametersTypes = new List<Type>(checkedTypes);

            var constructorsParameters =
                type.GetConstructors()
                    .SelectMany(GetConstructorRefParameters)
                    .Where(p=>p != type)
                    .Distinct();

            foreach (var parameterType in constructorsParameters)
            {
                if (!IsChecked(parameterType, parametersTypes))
                {
                    parametersTypes.Add(parameterType);

                    if (!parameterType.IsInterface)
                    {
                        var newParameterTypes =
                            GetConstructorsParametersTypes(parameterType, implementationTypes, parametersTypes)
                                .Where(p => !IsChecked(p, parametersTypes));

                        parametersTypes.AddRange(newParameterTypes);
                    }
                    else
                    {
                        var inheritingClasses = AllRunningRelevantTypes
                            .Where(p => parameterType.IsAssignableFrom(p)).ToList();

                        if (!implementationTypes.TryGetValue(parameterType, out var inheritingClass)
                            && inheritingClasses.Count == 1)
                            inheritingClass = inheritingClasses[0];

                        if (inheritingClass != null)
                        {
                            var interfaceImplementedParameterTypes = GetConstructorsParametersTypes(inheritingClass, implementationTypes, parametersTypes)
                                .Where(p => !IsChecked(p, parametersTypes));

                                parametersTypes.AddRange(interfaceImplementedParameterTypes);
                        }
                    }
                }
            }

            return parametersTypes.Distinct().ToList();
        }


        private static bool IsChecked(Type type, ICollection<Type> checkedTypes)
        {
            return checkedTypes.Contains(type);
        }

    }
}
