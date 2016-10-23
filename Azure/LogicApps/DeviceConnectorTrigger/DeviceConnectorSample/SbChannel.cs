using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;

namespace DeviceConnectorSample
{
    public class SbChannel : IDataChannel
    {
        private string m_SbConnStr ;

        private string m_QueuePath = "samplequeue";

        public SbChannel(string sbConnStr, string queuePath)
        {
            if (sbConnStr != null)
                m_SbConnStr = sbConnStr;

            if (queuePath != null)
                m_QueuePath = queuePath;
        }

        public List<Command> GetTelemetryData()
        {
            DateTime startTime = DateTime.Now;

            List<Command> commands = new List<Command>();

            var queueClient = QueueClient.CreateFromConnectionString(m_SbConnStr, m_QueuePath, ReceiveMode.ReceiveAndDelete);

            while (DateTime.Now - startTime < TimeSpan.FromSeconds(30))
            {
                var msgs = queueClient.ReceiveBatch(100, TimeSpan.FromSeconds(15));
                if (msgs != null)
                {
                    foreach (var msg in msgs)
                    {
                        try
                        {
                            Command cmd = msg.GetBody<Command>(new DataContractJsonSerializer(typeof(Command)));

                            //
                            // Depending on scenario we can send command in body or as properties.
                            if (cmd.DeviceId == null && cmd.DeviceName == null)
                            {
                                if (msg.Properties.ContainsKey("DeviceId"))
                                    cmd.DeviceId = msg.Properties["DeviceId"] as string;
                                if (msg.Properties.ContainsKey("DeviceName"))
                                    cmd.DeviceName = msg.Properties["DeviceName"] as string;
                                if (msg.Properties.ContainsKey("Temperature"))
                                {
                                    if (msg.Properties["Temperature"].GetType() == typeof(Int32))
                                        cmd.Temperature = (Int32)msg.Properties["Temperature"];
                                    else
                                        cmd.Temperature = (double)(Int32)msg.Properties["Temperature"];
                                }

                                if (msg.Properties.ContainsKey("Timestamp"))
                                    cmd.Timestamp = (string)msg.Properties["Timestamp"];
                            }

                            commands.Add(cmd);
                        }
                        catch (Exception) { };
                    }
                }
            }

            return commands;
        }
    }
}