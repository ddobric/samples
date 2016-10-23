using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceConnectorSample
{

    public class Command
    {
        public string DeviceId { get; set; }

        public string DeviceName { get; set; }

        public double Temperature { get; set; }

        public string Timestamp { get; set; }

        public string Id { get; set; }

    }
}