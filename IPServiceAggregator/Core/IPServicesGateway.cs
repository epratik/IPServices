using IPServiceAggregator.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPServiceAggregator.Core
{
    public class IPServicesGateway : IIPServicesGateway
    {
        IConfiguration config;
        IIPServicesFactory serviceFactory;
        string defaultServices;
        public IPServicesGateway(IConfiguration config, IIPServicesFactory serviceFactory)
        {
            this.config = config;
            this.serviceFactory = serviceFactory;
            defaultServices = config["defaultServices"];
        }

        public async Task<Result[]> AggregateResults(string services, string ip)
        {
            string[] servArray;
            List<Task<Result>> lstTasks = new List<Task<Result>>();
            Result[] results = null;

            if (services == null)
                servArray = defaultServices.Split(',');
            else
                servArray = services.Split(',');

            foreach (string service in servArray)
            {
                var instance = serviceFactory.GetInstance(service);
                Task<Result> task = instance.GetResultAsync(ip);
                lstTasks.Add(task);
            }

            Task<Result[]> allTasks = Task.WhenAll(lstTasks);

            results = await allTasks;

            return results;
        }
    }
}
