namespace EasyMoq.Tests.ListOfDependenciesBug.TestClasses
{
    public interface ITransportationFactory
    {
        ITransportationProvider GetTransportationProvider(TransportationType transportationType);
    }
}