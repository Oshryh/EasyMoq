using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EasyMoq.Tests.TestModelClasses.ForIntegrationTestMockBuilderTests
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<DbProviderClass>(),
                Component.For<IExternalSupplierClass>().ImplementedBy<ExternalSupplierClass>(),
                Component.For<IInternalLogicClass>().ImplementedBy<InternalLogicClass>(),
                Component.For<ITestIntegrationApp>().ImplementedBy<TestIntegrationApp>(),
                Component.For<ILoggerClass>().ImplementedBy<LoggerClass>()
            );
        }
    }
}
