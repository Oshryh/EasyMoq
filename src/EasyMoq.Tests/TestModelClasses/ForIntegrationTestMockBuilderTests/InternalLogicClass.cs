namespace EasyMoq.Tests.TestModelClasses.ForIntegrationTestMockBuilderTests
{
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
}