using IPServiceAggregator.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPServiceAggregator.Core
{
    public class IPServicesFactory: IIPServicesFactory
    {
        private IConfiguration config;
        public IPServicesFactory(IConfiguration config)
        {
            this.config = config;
        }
        public IIPBasedService GetInstance(string serviceName)
        {
            if (serviceName.ToLower() == "geoip")
                return new GeoIP(config);
            else if (serviceName.ToLower() == "rdap")
                return new RDAP(config);
            else if (serviceName.ToLower() == "ping")
                return new PingService(config);
            else
                return null;
        }
    }
}
