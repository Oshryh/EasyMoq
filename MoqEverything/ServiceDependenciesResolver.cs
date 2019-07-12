using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace MoqEverything
{
    public class ServiceDependenciesResolver<T> : IWindsorInstaller where T : class
    {
        private readonly Dictionary<Type, Type> _implementationTypes;
        private readonly TypeHelpers _typeHelpers;
        private readonly TypeMocker _typeMocker;

        public ServiceDependenciesResolver(Dictionary<Type, Type> implementationTypes, TypeHelpers typeHelpers, TypeMocker typeMocker)
        {
            _implementationTypes = implementationTypes;
            _typeHelpers = typeHelpers;
            _typeMocker = typeMocker;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var parametersTypesToRegister = _typeHelpers.GetConstructorsParametersTypes(typeof(T), _implementationTypes);

            var kernel = container.Kernel;
            kernel.Resolver.AddSubResolver(new AutoMoqResolver(kernel, parametersTypesToRegister));

            _typeMocker.RegisterTypes(container, parametersTypesToRegister, _implementationTypes);
        }
    }
}