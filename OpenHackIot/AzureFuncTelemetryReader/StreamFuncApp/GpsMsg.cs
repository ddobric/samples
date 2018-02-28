using System;
using System.Collections.Generic;
using System.Text;

namespace StreamReaderFunctionApp
{

    public class GpsMsg
    {
        public string rideId { get; set; }

        public string trainId { get; set; }

        public string correlationId { get; set; }

        public double lat { get; set; }

        public double @long { get; set; }

        public double alt { get; set; }

        public double speed { get; set; }

        public double vertAccuracy { get; set; }

        public double horizAccuracy { get; set; }

        public string deviceTime { get; set; }
    }
}
