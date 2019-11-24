using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPServiceAggregator.DTO
{
    public class ServiceInput
    {
        /// <summary>
        /// valid ip address
        /// </summary>
        [FromRoute(Name = "IpAddress")]
        public string IpAddress { get; set; }
        /// <summary>
        /// comma separated services ex - geoip,rdap. If kept blank default list will be considered.
        /// </summary>
        [FromQuery(Name = "Services")]
        public string Services { get; set; }
    }
}
