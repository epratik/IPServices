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

namespace RDAPWorkerService
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
            path = configuration["rdap"];
            this.multiplexer = multiplexer;
            this.consumer = consumer;
            semaphoreSlim = new SemaphoreSlim(int.Parse(configuration["MaxThreads"]));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                consumer.Subscribe("rdap");
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
            catch(Exception ex)
            {
                logger.LogError(ex, "RDAP service call exception", null);
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
                db.HashSet(ip, new HashEntry[] { new HashEntry("rdap", "hello world") });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RDAP service call exception", null);
                db.HashSet(ip, new HashEntry[] { new HashEntry("rdap", "Error-RDAP Service Failed") });
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
