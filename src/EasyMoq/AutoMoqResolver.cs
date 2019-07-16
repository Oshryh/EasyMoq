using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Moq;

namespace EasyMoq
{
    public class AutoMoqResolver : ISubDependencyResolver
    {
        private readonly IKernel _kernel;
        private readonly IList<Type> _registeredTypes;

        public AutoMoqResolver(IKernel kernel, IList<Type> constructorsParameters)
        {
            _kernel = kernel;
            _registeredTypes = constructorsParameters;
        }

        public void AddRegisteredType(Type type)
        {
            _registeredTypes.Add(type);
        }

        public void AddRegisteredType<T>() where T : Type
        {
            _registeredTypes.Add(typeof(T));
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            var canResolve = _registeredTypes.Contains(dependency.TargetType);

            return canResolve;
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            var mockType = typeof(Mock<>).MakeGenericType(dependency.TargetType);
            return ((Mock)_kernel.Resolve(mockType)).Object;
        }
    }
}
