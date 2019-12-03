using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace GeoIpWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private IConfiguration configuration;
        private IConnectionMultiplexer multiplexer;
        private IConsumer<Null, string> consumer;
        private SemaphoreSlim semaphoreSlim;
        string path;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IConnectionMultiplexer multiplexer, IConsumer<Null, string> consumer)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.multiplexer = multiplexer;
            this.consumer = consumer;
            semaphoreSlim = new SemaphoreSlim(int.Parse(configuration["MaxThreads"]));
            path = configuration["geoip"];
        }

        /// <summary>
        /// Subscribes to geoip topic in kafka queue and loops infinitely checking for new messages.
        /// It will process only N messages at a time.
        /// Semaphore slim is used for throttling the number of threads. 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                consumer.Subscribe("geoip");
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
                logger.LogError(ex, "GEOIP service call exception", null);
            }

        }

        private async Task Process(string ip)
        {
            var db = multiplexer.GetDatabase();
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(string.Format(path, ip));
                var json = response.Content.ReadAsStringAsync().Result;
                db.HashSet(ip, new HashEntry[] { new HashEntry("geoip", json) }) ;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GeoIp service call exception", null);
                db.HashSet(ip, new HashEntry[] { new HashEntry("geoip", "Error-GEOIP Service Failed") });
            }
        }
    }
}
