using IPServiceAggregator.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IPServiceAggregator.Core
{
    public class GeoIP : IIPBasedService
    {
        private IConfiguration config;
        private ILogger<GeoIP> logger;
        string path;
        public GeoIP(IConfiguration config, ILogger<GeoIP> logger)
        {
            this.config = config;
            this.logger = logger;
            path = config["geoip"];
        }
        public async Task<Result> GetResultAsync(string ip)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(string.Format(path, ip));
                return new Result()
                {
                    Output = response.Content.ReadAsAsync<JObject>().Result,
                    Service = "GeoIP"
                };
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "GeoIP service call exception", null);
                return new Result() { Output = JObject.Parse("{ Error: \"GeoIP Service Failed.\" }"), Service = "GeoIP" };
            }
        }
    }
}
