using IPServiceAggregator.Interfaces;
using Microsoft.Extensions.Configuration;
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
        string path;
        public GeoIP(IConfiguration config)
        {
            this.config = config;
            path = config["geoip"];
        }
        public async Task<Result> GetResultAsync(string ip)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(string.Format(path, ip));
            return new Result()
            {
                Output = response.Content.ReadAsAsync<JObject>().Result,
                Service = "GeoIP"
            };
        }
    }
}
