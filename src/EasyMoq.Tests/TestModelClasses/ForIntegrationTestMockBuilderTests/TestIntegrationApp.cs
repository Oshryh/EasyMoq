namespace EasyMoq.Tests.TestModelClasses.ForIntegrationTestMockBuilderTests
{
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
