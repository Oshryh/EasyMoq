using System;
using System.Collections.Generic;
using Castle.MicroKernel;

namespace EasyMoq.Interfaces
{
    public interface IAutoMoqResolver : ISubDependencyResolver
    {
        void AddRegisteredType(Type type);
        void AddRegisteredType<T>() where T : Type;
        void AddRegisteredTypeRange(IEnumerable<Type> types);
    }
}