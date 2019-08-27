namespace EasyMoq.Examples.Example3_IntegrationTestMock_WithDefaultInstanceBehavior
{
    public interface ILoggerClass
    {
        string LogMessage();
    }

    public class LoggerClass : ILoggerClass
    {
        public string LogMessage()
        {
            return "Logged a message";
        }
    }

    public interface IExternalSupplierClass
    {
        string GetDataFromUnreliableSupplier();
    }

    class ExternalSupplierClass : IExternalSupplierClass
    {
        public string GetDataFromUnreliableSupplier()
        {
            return "Get data from unreliable supplier.";
        }
    }

    public class DbProviderClass
    {
        public string GetOtherDataFromDb()
        {
            return "Other data from DB";
        }

        public string GetDataFromDb()
        {
            return "Data from DB";
        }
    }

    public interface IInternalLogicClass
    {
        string GetInfoFromExternalSupplierAndDb();
    }

    class InternalLogicClass : IInternalLogicClass
    {
        private readonly IExternalSupplierClass _externalSupplier;
        private readonly DbProviderClass _dbProvider;

        public InternalLogicClass(IExternalSupplierClass externalSupplier, DbProviderClass dbProvider)
        {
            _externalSupplier = externalSupplier;
            _dbProvider = dbProvider;
        }

        public string GetInfoFromExternalSupplierAndDb()
        {
            return _externalSupplier.GetDataFromUnreliableSupplier() + _dbProvider.GetDataFromDb();
        }
    }

    public interface ITestIntegrationApp
    {
        string DoStuffWithServices();
    }

    public class TestIntegrationApp : ITestIntegrationApp
    {
        private readonly DbProviderClass _dbProvider;
        private readonly IInternalLogicClass _internalLogic;
        private readonly ILoggerClass _logger;

        public TestIntegrationApp(DbProviderClass dbProvider, IInternalLogicClass internalLogic, ILoggerClass logger)
        {
            _dbProvider = dbProvider;
            _internalLogic = internalLogic;
            _logger = logger;
        }

        public string DoStuffWithServices()
        {
            return _logger.LogMessage()
                   + _internalLogic.GetInfoFromExternalSupplierAndDb()
                   + _dbProvider.GetOtherDataFromDb();
        }
    }

}
