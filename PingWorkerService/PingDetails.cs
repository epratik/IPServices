using System;
using System.Collections.Generic;
using System.Text;

namespace PingWorkerService
{
    public class PingDetails
    {
        public int PacketNo { get; set; }
        public string Status { get; set; }
        public long RoundtripTime { get; set; }
    }
}
