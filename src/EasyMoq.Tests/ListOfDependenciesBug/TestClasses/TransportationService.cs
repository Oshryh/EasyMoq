namespace EasyMoq.Tests.ListOfDependenciesBug.TestClasses
{
    public class TransportationService : ITransportationService
    {
        private readonly ITransportationFactory _transportationFactory;

        public TransportationService(ITransportationFactory transportationFactory)
        {
            _transportationFactory = transportationFactory;
        }

        public string GetTransportation(TransportationType transportationType)
        {
            return _transportationFactory
                .GetTransportationProvider(transportationType)
                .GetFormOfTransportation();
        }
    }
}