using System;
using System.Collections.Generic;
using System.Text;

namespace EasyMoq.Tests.ListOfDependenciesBug.TestClasses
{
    public interface ITransportationService
    {
        string GetTransportation(TransportationType transportationType);

    }
}
