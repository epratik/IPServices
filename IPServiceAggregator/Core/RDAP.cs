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
    public class RDAP : IIPBasedService
    {
        private IConfiguration config;
        private string path;
        public RDAP(IConfiguration config)
        {
            this.config = config;
            path = config["rdap"];
        }
        public async Task<Result> GetResultAsync(string ip)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(string.Format(path,ip));
            var json = response.Content.ReadAsStringAsync().Result;

            return new Result()
            {
                Output = JObject.Parse(json),
                Service = "RDAP"
            };
        }
    }
}
