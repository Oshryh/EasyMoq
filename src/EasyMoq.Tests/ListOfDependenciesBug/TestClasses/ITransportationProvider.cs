namespace EasyMoq.Tests.ListOfDependenciesBug.TestClasses
{
    public interface ITransportationProvider
    {
        TransportationType ProviderType { get; }

        string GetFormOfTransportation();
    }
}
