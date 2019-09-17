using System.Collections.Generic;
using System.Linq;

namespace EasyMoq.Tests.ListOfDependenciesBug.TestClasses
{
    public class TransportationFactory : ITransportationFactory
    {
        private readonly IList<ITransportationProvider> _transportationProvider;

        public TransportationFactory(IList<ITransportationProvider> transportationProvider)
        {
            _transportationProvider = transportationProvider;
        }

        public ITransportationProvider GetTransportationProvider(TransportationType transportationType)
        {
            return _transportationProvider.FirstOrDefault(p => p.ProviderType == transportationType);
        }
    }
}
