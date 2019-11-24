using IPServiceAggregator.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPServiceAggregator.Interfaces
{
    public interface IIPBasedService
    {
        Task<Result> GetResultAsync(string ip);
    }
}
