using IPServiceAggregator.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IPServiceAggregator.Core
{
    public class IPServicesFactory: IIPServicesFactory
    {
        private IConfiguration config;
        private IHttpContextAccessor httpContextAccessor;
        public IPServicesFactory(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this.config = config;
            this.httpContextAccessor = httpContextAccessor;
        }
        public IIPBasedService GetInstance(string serviceName)
        {
            if (serviceName.ToLower() == "geoip")
                return new GeoIP(config, httpContextAccessor.HttpContext.RequestServices.GetService<ILogger<GeoIP>>());
            else if (serviceName.ToLower() == "rdap")
                return new RDAP(config, httpContextAccessor.HttpContext.RequestServices.GetService<ILogger<RDAP>>());
            else if (serviceName.ToLower() == "ping")
                return new PingService(config, httpContextAccessor.HttpContext.RequestServices.GetService<ILogger<PingService>>());
            else
                return null;
        }
    }
}
