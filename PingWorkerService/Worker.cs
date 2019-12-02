using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace PingWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private IConfiguration configuration;
        private IConnectionMultiplexer multiplexer;
        private IConsumer<Null, string> consumer;
        private SemaphoreSlim semaphoreSlim;
        int noOfPackets = 3;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IConnectionMultiplexer multiplexer, IConsumer<Null, string> consumer)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.multiplexer = multiplexer;
            this.consumer = consumer;
            semaphoreSlim = new SemaphoreSlim(int.Parse(configuration["MaxThreads"]));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                consumer.Subscribe("ping");
                while (!stoppingToken.IsCancellationRequested)
                {
                    var message = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (message != null)
                    {
                        Console.WriteLine(message.Value);
                        await semaphoreSlim.WaitAsync();
                        Task task = Process(message.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PING service call exception", null);
            }

        }

        private async Task Process(string ip)
        {
            var db = multiplexer.GetDatabase();
            try
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

                db.HashSet(ip, new HashEntry[] { new HashEntry("ping", JsonSerializer.Serialize(pingDetails)) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ping service call exception", null);
                db.HashSet(ip, new HashEntry[] { new HashEntry("ping", "Error-PING Service Failed") });
            }
        }
    }
}
