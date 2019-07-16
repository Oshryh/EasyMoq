using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moq;

namespace EasyMoq
{
    /// <summary>
    /// Base class which provides full recursive mocking of all the dependent classes, and methods to help with testing.
    /// </summary>
    /// <typeparam name="TService">The service being test.</typeparam>
    /// <typeparam name="TIService">The interface of the service which methods are tested.</typeparam>
    public abstract class BaseServiceTest<TIService, TService> : IDisposable
        where TIService : class
        where TService : class, TIService
    {
        private IWindsorContainer Container { get; }
        private readonly Dictionary<Type, Type> _implementationTypes = new Dictionary<Type, Type>();
        private AutoMoqResolver _autoMoqSubResolver;

        private readonly TypeHelpers _typeHelpers;
        private readonly TypeMocker _typeMocker;

        protected BaseServiceTest()
        {
            _typeHelpers = new TypeHelpers();
            _typeMocker = new TypeMocker(_typeHelpers);
            Container = new WindsorContainer();

            RegisterAllDependencies<TIService, TService>();

            Prepare();

            Container.Register(
                Component.For<TIService>()
                    .Instance((TIService)_typeMocker.RegisterMockAndGetInstance(Container, typeof(TService), typeof(TIService))));
        }

        protected virtual void Prepare() { }

        protected void CoupleInterfaceWithClass<TInterface, TClass>()
        {
            _implementationTypes.Add(typeof(TInterface), typeof(TClass));
        }

        protected void AddAssemblyNamePartFilter(string assemblyNamePartFilter)
        {
            _typeHelpers.AssembliesNamesParts.Add(assemblyNamePartFilter);
        }

        protected Mock<T> AddStaticOfMockToContainer<T>() where T : class
        {
            var mockOfStatic = StaticMockOf<T>.Get();
            Container.Register(Component.For<Mock<T>>().Instance(mockOfStatic));

            _autoMoqSubResolver.AddRegisteredType(typeof(T));

            Container.Register(
                Component.For<object>()
                    .Named(GetNameOfLockObjectOf<T>())
                    .Instance(StaticMockOf<T>.SettingsLock));

            return mockOfStatic;
        }

        protected object GetStaticLockObjectOfMockOf<T>() where T : class
        {
            var nameOfLockObject = GetNameOfLockObjectOf<T>();

            if (!Container.Kernel.HasComponent(nameOfLockObject))
                AddStaticOfMockToContainer<T>();

            return Container.Resolve<object>(nameOfLockObject);
        }

        private static string GetNameOfLockObjectOf<T>() where T : class
        {
            return "Lock" + nameof(T);
        }

        protected TIService GetTestedService()
        {
            var service = Container.Resolve<TIService>();
            return service;
        }

        protected Mock<TIService> GetTestedMockService()
        {
            var service = Container.Resolve<Mock<TIService>>();
            return service;
        }

        protected Mock<T> GetRelatedMock<T>()
            where T : class
        {
            var mock = Container.Resolve<Mock<T>>();
            return mock;
        }

        protected void ReleaseMock<TInterface>()
            where TInterface : class
        {
            Container.Release(GetRelatedMock<TInterface>());
        }

        private void RegisterAllDependencies<TInterface, TClass>()
            where TInterface : class
            where TClass : class, TInterface
        {
            var parametersTypesToRegister =
                _typeHelpers.GetConstructorsParametersTypes(typeof(TClass), _implementationTypes);

            _autoMoqSubResolver = new AutoMoqResolver(Container.Kernel, parametersTypesToRegister);
            Container.Kernel.Resolver.AddSubResolver(_autoMoqSubResolver);
            _typeMocker.RegisterTypes(Container, parametersTypesToRegister, _implementationTypes);
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}