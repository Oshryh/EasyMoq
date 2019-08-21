namespace EasyMoq.Examples.Example2_IntegrationTestMock_OneMethodMockedAnd1RunningNormally
{
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

        public TestIntegrationApp(DbProviderClass dbProvider, IInternalLogicClass internalLogic)
        {
            _dbProvider = dbProvider;
            _internalLogic = internalLogic;
        }

        public string DoStuffWithServices()
        {
            return _internalLogic.GetInfoFromExternalSupplierAndDb()
                   + _dbProvider.GetOtherDataFromDb();
        }

    }
}
