using IPServiceAggregator.Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPServiceAggregator.Interfaces
{
    public interface IIPServicesGateway
    {
        Task<HashEntry[]> AggregateResults(string services, string ip);
    }
}
