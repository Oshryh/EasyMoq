using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EasyMoq.Examples.Example2_IntegrationTestMock_OneMethodMockedAnd1RunningNormally
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<DbProviderClass>(),
                Component.For<IExternalSupplierClass>().ImplementedBy<ExternalSupplierClass>(),
                Component.For<IInternalLogicClass>().ImplementedBy<InternalLogicClass>(),
                Component.For<ITestIntegrationApp>().ImplementedBy<TestIntegrationApp>()
            );
        }
    }
}
