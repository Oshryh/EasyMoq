using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyMoq
{
    public class AutoMoqResolver : ISubDependencyResolver
    {
        private readonly IKernel _kernel;
        private readonly List<Type> _registeredTypes = new List<Type>();

        public AutoMoqResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void AddRegisteredType(Type type)
        {
            if (!_registeredTypes.Contains(type))
                _registeredTypes.Add(type);
        }

        public void AddRegisteredTypeRange(IEnumerable<Type> types)
        {
            types.ToList().ForEach(AddRegisteredType);
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
