using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPServiceAggregator.Interfaces
{
    public interface IIPServicesFactory
    {
        IIPBasedService GetInstance(string serviceName);
    }
}
