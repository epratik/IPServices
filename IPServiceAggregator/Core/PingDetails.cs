using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPServiceAggregator.Core
{
    public class PingDetails
    {
        public int PacketNo { get; set; }
        public string Status { get; set; }
        public long RoundtripTime { get; set; }
    }
}
