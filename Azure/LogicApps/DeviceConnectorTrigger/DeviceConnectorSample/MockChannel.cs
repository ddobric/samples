using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceConnectorSample
{
    public class MockChannel : IDataChannel
    {
        public List<Command> GetTelemetryData()
        {
            List<Command> commands = new List<Command>();

            for (int i = 0; i < 20; i++)
            {
                commands.Add(new Command()
                    {
                        DeviceId = String.Format("TMPSENSOR{0}", i),
                        DeviceName = String.Format("Pi Temperature sensor {0}", i),
                        Id = Guid.NewGuid().ToString(),
                        Temperature = DateTime.Now.Second,
                        Timestamp = DateTime.Now.ToString(),
                    });

            }

            return commands;
        }
    }
}