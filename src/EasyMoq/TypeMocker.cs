using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moq;

namespace EasyMoq
{
    public class TypeMocker
    {
        private readonly TypeHelpers _typeHelpers;
        private readonly List<Type> _registeredParametersTypes;

        public TypeMocker(TypeHelpers typeHelpers)
        {
            _typeHelpers = typeHelpers;
            _registeredParametersTypes = new List<Type>();
        }

        public void RegisterTypes(IWindsorContainer container, List<Type> parametersTypesToRegister, Dictionary<Type, Type> implementationTypes)
        {
            foreach (var parameterType in parametersTypesToRegister.Where(p => p.IsInterface))
            {
                var inheritingClass = GetInterfaceSingleImplementingClass(implementationTypes, parameterType);

                if (!_registeredParametersTypes.Contains(parameterType))
                {
                    _registeredParametersTypes.Add(parameterType);

                    if (inheritingClass != null && inheritingClass != parameterType)
                        RegisterMockAndGetInstance(container, inheritingClass, parameterType);
                    else
                        container.Register(Component.For(typeof(Mock<>).MakeGenericType(parameterType)));
                }
            }

            foreach (var parameterType in parametersTypesToRegister)
                RegisterTypeInstance(container, parameterType);
        }

        private Type GetInterfaceSingleImplementingClass(IReadOnlyDictionary<Type, Type> implementationTypes, Type parameterType)
        {
            if (!implementationTypes.TryGetValue(parameterType, out var inheritingClass)
                && TryGetSingleImplementingClassType(parameterType, out var singleInheritingClassType))
                inheritingClass = singleInheritingClassType;

            return inheritingClass;
        }

        private bool TryGetSingleImplementingClassType(Type interfaceType, out Type singleInheritingClassType)
        {
            var inheritingClassTypes = _typeHelpers.AllRunningRelevantTypes
                .Where(interfaceType.IsAssignableFrom).ToList();

            singleInheritingClassType = inheritingClassTypes.Count == 1
                ? inheritingClassTypes.SingleOrDefault()
                : null;

            return singleInheritingClassType == null;
        }

        private object RegisterTypeInstance(IWindsorContainer container, Type parameterType)
        {
            if (_registeredParametersTypes.Contains(parameterType))
                return ((Mock)container.Resolve(typeof(Mock<>).MakeGenericType(parameterType))).Object;

            _registeredParametersTypes.Add(parameterType);

            return RegisterMockAndGetInstance(container, parameterType);
        }

        public object RegisterMockAndGetInstance(IWindsorContainer container, Type parameterType)
        {
            return RegisterMockAndGetInstance(container, parameterType, parameterType);
        }

        public object RegisterMockAndGetInstance(IWindsorContainer container, Type parameterType, Type parameterInterfaceType)
        {
            var constructorParametersInstances = ConstructorParametersInstancesArray(container, parameterType);

            var mockedType = typeof(Mock<>).MakeGenericType(parameterType);
            var newMockInstance = Activator.CreateInstance(mockedType, constructorParametersInstances);
            var mockedInterfaceType = typeof(Mock<>).MakeGenericType(parameterInterfaceType);

            if (parameterInterfaceType != parameterType)
            {
                newMockInstance = typeof(Mock<>).GetMethod(nameof(Mock.As))
                    .MakeGenericMethod(parameterInterfaceType)
                    .Invoke(newMockInstance, new object[0]);
            }

            ((Mock)newMockInstance).CallBase = true;
            container.Register(Component.For(mockedInterfaceType).Instance(newMockInstance));

            return ((Mock)newMockInstance).Object;
        }

        private object[] ConstructorParametersInstancesArray(IWindsorContainer container, Type parameterType)
        {
            var constructorsParametersTypes = parameterType.GetConstructors()
                .Select(c => _typeHelpers.GetConstructorRefParameters(c))
                .Where(p => !p.Contains(parameterType))
                .ToList();

            if (constructorsParametersTypes.Any())
            {
                var constructorParametersTypes = constructorsParametersTypes.OrderByDescending(p => p.Count).FirstOrDefault();

                var constructorParametersInstances = constructorParametersTypes
                    .Select(p => RegisterTypeInstance(container, p));

                return constructorParametersInstances.ToArray();
            }

            return new object[0];
        }
    }
}