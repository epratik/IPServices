using Confluent.Kafka;
using IPServiceAggregator.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPServiceAggregator.Core
{
    public class IPServicesGateway : IIPServicesGateway
    {
        IConfiguration config;
        IProducer<Null, string> producer;
        string defaultServices;
        IConnectionMultiplexer multiplexer;
     
        int retryCount;

        public IPServicesGateway(IConfiguration config, IProducer<Null,string> producer, IConnectionMultiplexer multiplexer)
        {
            //Use Ioptions
            this.config = config;
            defaultServices = config["defaultServices"];
            retryCount = int.Parse(config["RetryCount"]);
            this.producer = producer;
            this.multiplexer = multiplexer;

        }

        public async Task<HashEntry[]> AggregateResults(string services, string ip)
        {
            string[] inpSerArray;
            List<Task<DeliveryResult<Null, string>>> lstTasks = new List<Task<DeliveryResult<Null, string>>>();

            if (services == null)
                inpSerArray = defaultServices.Split(',');
            else
                inpSerArray = services.Split(',');

            var db = multiplexer.GetDatabase();
            
            var servicesInCache = db.HashKeys(ip);//empty if key does not exist.
            var servicesToCall = inpSerArray.Except(servicesInCache.Select(x => x.ToString()));

            foreach (string service in servicesToCall)
            {
                Task<DeliveryResult<Null, string>> task = producer.ProduceAsync(service, new Message<Null, string>() { Key = null, Value = ip });
                lstTasks.Add(task);
            }

            await Task.WhenAll(lstTasks);

            var allEntries = await GetRemainingEntries(servicesInCache.Count(), servicesToCall.Count(), ip);
            return allEntries.Where(x => inpSerArray.Any(y => y == x.Name)).ToArray();
        }

        private async Task<HashEntry[]> GetRemainingEntries(int noOfServicesInCache, int noOfServicesToCall, string ip)
        {
            var db = multiplexer.GetDatabase();
            while (retryCount != 0 && noOfServicesToCall > 0)
            {
                await Task.Delay(1000);
                if (db.HashLength(ip) == noOfServicesInCache + noOfServicesToCall)
                    break;
                else
                    retryCount--;
            }
           return db.HashGetAll(ip);
        }
    }
}
