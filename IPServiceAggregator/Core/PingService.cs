using IPServiceAggregator.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace IPServiceAggregator.Core
{
    public class PingService : IIPBasedService
    {
        //private IConfiguration config;
        private ILogger<PingService> logger;
        private const int noOfPackets = 2;
        public PingService(ILogger<PingService> logger)
        {
            //this.config = config;
            this.logger = logger;

        }
        public async Task<Result> GetResultAsync(string ip)
        {

            IPAddress ipAddress;
            List<PingDetails> pingDetails = new List<PingDetails>();
            IPAddress.TryParse(ip, out ipAddress);
            using (Ping ping = new Ping())
                for (int i = 0; i < noOfPackets; i++)
                {
                    PingReply reply = await ping.SendPingAsync(ipAddress, 1000);
                    pingDetails.Add(new PingDetails() { PacketNo = i, RoundtripTime = reply.RoundtripTime, Status = reply.Status.ToString() });
                };
            var jsonPingDet = new JObject { ["Packets"] = JToken.FromObject(pingDetails) };
            var result = new Result() { Output = jsonPingDet, Service = "PING" };

            return null;
        }
    }
}
