using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceConnectorSample
{
    public interface IDataChannel
    {
        List<Command> GetTelemetryData();
       
    }
}