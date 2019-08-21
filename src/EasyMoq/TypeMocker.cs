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
        private readonly IWindsorContainer _container;
        private readonly Type _typeOfGenericMock;
        private readonly MockStrategy _mockStrategy;

        public TypeMocker(IWindsorContainer container, TypeHelpers typeHelpers, MockStrategy mockStrategy)
        {
            _container = container;
            _typeHelpers = typeHelpers;
            _mockStrategy = mockStrategy;

            _registeredParametersTypes = new List<Type>();
            _typeOfGenericMock = typeof(Mock<>);
        }

        public void RegisterTypes(List<Type> parametersTypesToRegister, IReadOnlyDictionary<Type, Type> implementationTypes)
        {
            parametersTypesToRegister.ForEach(parameterType => RegisterType(implementationTypes, parameterType));
        }

        public void RegisterMockWithInstanceFactory(Type parameterType)
        {
            RegisterMockWithInstanceFactory(parameterType, parameterType);
        }

        public void RegisterMockWithInstanceFactory(Type parameterType, Type parameterInterfaceType)
        {
            if (IsRegistered(parameterType, parameterInterfaceType))
                return;

            var mockedInterfaceType = _typeOfGenericMock.MakeGenericType(parameterInterfaceType);

            var mockedType = parameterInterfaceType != parameterType
                ? _typeOfGenericMock.MakeGenericType(parameterType)
                : mockedInterfaceType;

            _container.Register(Component.For(mockedInterfaceType).UsingFactoryMethod(_ =>
            {
                var constructorParametersInstances = ConstructorParametersInstancesArray(parameterType);
                var newMockInstance = Activator.CreateInstance(mockedType, constructorParametersInstances);

                if (parameterInterfaceType != parameterType)
                {
                    newMockInstance = _typeOfGenericMock.GetMethod(nameof(Mock.As))
                        .MakeGenericMethod(parameterInterfaceType)
                        .Invoke(newMockInstance, new object[0]);
                }

                ((Mock) newMockInstance).CallBase = true;
                return newMockInstance;
            }));

            _registeredParametersTypes.Add(parameterType);
        }

        private void RegisterType(IReadOnlyDictionary<Type, Type> implementationTypes, Type parameterType)
        {
            if (parameterType.IsInterface)
                RegisterMockInterfaceWithImplementationIfFound(parameterType, implementationTypes);
            else
                RegisterMockWithInstanceFactory(parameterType);
        }

        private void RegisterMockInterfaceWithImplementationIfFound(Type parameterType, IReadOnlyDictionary<Type, Type> implementationTypes)
        {
            var inheritingClass = GetInterfaceSingleImplementingClass(implementationTypes, parameterType);

            if (inheritingClass != null && inheritingClass != parameterType)
                RegisterMockWithInstanceFactory(inheritingClass, parameterType);
            else
                RegisterMockInterface(parameterType);
        }

        private void RegisterMockInterface(Type parameterType)
        {
            if (IsRegistered(parameterType))
                return;

            _container.Register(Component.For(_typeOfGenericMock.MakeGenericType(parameterType)));
            _registeredParametersTypes.Add(parameterType);
        }

        private bool IsRegistered(Type parameterType, Type parameterInterfaceType)
        {
            return IsRegistered(parameterType) || IsRegistered(parameterInterfaceType);
        }

        private bool IsRegistered(Type parameterType)
        {
            return _registeredParametersTypes.Contains(parameterType);
        }

        public object GetInstanceOfMockOfStaticOf(Type type)
        {
            var rawGenericType = typeof(StaticMockOf<>);
            var genericTypeOfType = rawGenericType.MakeGenericType(type);
            var instancePropertyOfStaticClass = genericTypeOfType.GetProperty("Instance");
            var getMethodOfInstanceProperty = instancePropertyOfStaticClass.GetGetMethod();
            var mockOfStatic = getMethodOfInstanceProperty.Invoke(genericTypeOfType, null);
            return mockOfStatic;
        }

        private Type GetInterfaceSingleImplementingClass(IReadOnlyDictionary<Type, Type> implementationTypes, Type parameterType)
        {
            if (!implementationTypes.TryGetValue(parameterType, out var inheritingClass)
                && _typeHelpers.TryGetSingleImplementingClassType(parameterType, out var singleInheritingClassType))
                inheritingClass = singleInheritingClassType;

            return inheritingClass;
        }

        private object GetRegisterTypeInstance(Type parameterType)
        {
            var desiredMock = _typeOfGenericMock.MakeGenericType(parameterType);

            if (_mockStrategy == MockStrategy.UnitTest)
                return ((Mock) _container.Resolve(desiredMock)).Object;
            else
                return _container.Kernel.HasComponent(desiredMock)
                    ? ((Mock) _container.Resolve(desiredMock)).Object
                    : _container.Resolve(parameterType);
        }

        private object[] ConstructorParametersInstancesArray(Type parameterType)
        {
            var constructorsParametersTypes = parameterType.GetConstructors()
                .Select(c => _typeHelpers.GetConstructorRefParameters(c))
                .Where(p => !p.Contains(parameterType))
                .ToList();

            if (constructorsParametersTypes.Any())
            {
                var constructorParametersTypes = constructorsParametersTypes.OrderByDescending(p => p.Count).First();
                var constructorParametersInstances = constructorParametersTypes.Select(GetRegisterTypeInstance);

                return constructorParametersInstances.ToArray();
            }

            return new object[0];
        }
    }
}