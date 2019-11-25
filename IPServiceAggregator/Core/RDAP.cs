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
    public class RDAP : IIPBasedService
    {
        private IConfiguration config;
        private ILogger<RDAP> logger;
        private string path;
        public RDAP(IConfiguration config, ILogger<RDAP> logger)
        {
            this.config = config;
            this.logger = logger;
            path = config["rdap"];
        }
        public async Task<Result> GetResultAsync(string ip)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(string.Format(path, ip));
                var json = response.Content.ReadAsStringAsync().Result;

                return new Result()
                {
                    Output = JObject.Parse(json),
                    Service = "RDAP"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RDAP service call exception", null);
                return new Result() { Output = JObject.Parse("{ Error: \"RDAP Service Failed.\" }"), Service = "RDAP" };
            }
        }
    }
}
