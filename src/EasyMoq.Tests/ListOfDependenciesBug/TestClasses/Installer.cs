using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EasyMoq.Tests.ListOfDependenciesBug.TestClasses
{
    class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new ListResolver(container.Kernel));

            container.Register(
                Component.For<ITransportationProvider>().ImplementedBy<TaxiProvider>(),
                Component.For<ITransportationProvider>().ImplementedBy<FlightProvider>(),
                Component.For<ITransportationProvider>().ImplementedBy<SailProvider>(),
                Component.For<ITransportationFactory>().ImplementedBy<TransportationFactory>(),
                Component.For<ITransportationService>().ImplementedBy<TransportationService>()
                    );
        }
    }
}
