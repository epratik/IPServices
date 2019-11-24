using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPServiceAggregator.DTO;
using IPServiceAggregator.Filters;
using IPServiceAggregator.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IPServiceAggregator
{
    [ApiController]
    [Route("api/[controller]")]
    public class IPController : Controller
    {
        private IIPServicesGateway gateway;
        public IPController(IIPServicesGateway gateway)
        {
            this.gateway = gateway;
        }

        /// <summary>
        /// Calls various services for the IP given. Supported services are - geoip, rdap and ping
        /// </summary>
        /// <returns></returns>
        [Route("{IpAddress}/services")]
        [RateLimitFilter(Seconds =10)]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute,FromQuery]ServiceInput inpParameters)
        {
            var result = await gateway.AggregateResults(inpParameters.Services, inpParameters.IpAddress);
            return Ok(result);
        }
    }
}
