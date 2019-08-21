namespace EasyMoq.Tests.TestModelClasses.ForIntegrationTestMockBuilderTests
{
    class ExternalSupplierClass : IExternalSupplierClass
    {
        public string GetDataFromUnreliableSupplier()
        {
            return "Get data from unreliable supplier.";
        }
    }
}