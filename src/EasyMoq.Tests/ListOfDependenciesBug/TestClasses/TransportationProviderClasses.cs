namespace EasyMoq.Tests.ListOfDependenciesBug.TestClasses
{
    public class TaxiProvider : ITransportationProvider
    {
        public TransportationType ProviderType => TransportationType.Taxi;

        public string GetFormOfTransportation()
        {
            return "Car";
        }
    }

    public class FlightProvider : ITransportationProvider
    {
        public TransportationType ProviderType => TransportationType.Flight;

        public string GetFormOfTransportation()
        {
            return "Plane";
        }
    }

    public class SailProvider : ITransportationProvider
    {
        public TransportationType ProviderType => TransportationType.Sail;

        public string GetFormOfTransportation()
        {
            return "Boat";
        }
    }
}
