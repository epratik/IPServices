using IPServiceAggregator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPServiceAggregator.Interfaces
{
    public interface IIPServicesGateway
    {
        Task<Result[]> AggregateResults(string services, string ip);
    }
}
