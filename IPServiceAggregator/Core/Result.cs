using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPServiceAggregator.Core
{
    public class Result
    {
        public string Service { get; set; }
        public JObject Output { get; set; }
    }
}
