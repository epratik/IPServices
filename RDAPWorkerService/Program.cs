using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace RDAPWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddOptions<HostOptions>().Configure(
                opts => opts.ShutdownTimeout = TimeSpan.FromMilliseconds(-1));
                    services.AddSingleton(typeof(IConnectionMultiplexer), x => ConnectionMultiplexer.Connect(hostContext.Configuration["RedisConn"]));
                    services.AddSingleton(typeof(IConsumer<Null, string>), x => new ConsumerBuilder<Null, String>(
                         new ConsumerConfig() { GroupId="rdap", BootstrapServers = hostContext.Configuration["KafkaConn"] }).Build());
                });
                
    }
}
