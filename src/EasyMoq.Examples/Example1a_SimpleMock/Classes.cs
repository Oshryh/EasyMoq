namespace EasyMoq.Examples.Example1a_SimpleMock
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

    public interface ILibraryClass
    {
        string GetInfoFromExternalSupplierAndDb();
    }

    public class LibraryClass : ILibraryClass
    {
        private readonly IExternalSupplierClass _externalSupplier;
        private readonly ILoggerClass _logger;

        public LibraryClass(IExternalSupplierClass externalSupplier, ILoggerClass logger)
        {
            _externalSupplier = externalSupplier;
            _logger = logger;
        }

        public string GetInfoFromExternalSupplierAndDb()
        {
            return $"Data from supplier: {_externalSupplier.GetDataFromUnreliableSupplier()}";
        }

        public string MethodWeAreNotTestingRightNow()
        {
            return $"Logger message: {_logger.LogMessage()}";
        }
    }

}
