using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceMessageReader
{
    class Program
    {
        static string m_SbConnStr;
        static string m_EventHubPath;
        static EventHubClient eventHubClient;

        static void Main(string[] args)
        {
            // Use this in a case of IoTHub
            //m_SbConnStr = ConfigurationManager.AppSettings["IotHub.ServiceConnStr"];
            //m_EventHubPath = ConfigurationManager.AppSettings["IotHub.EventHubPath"];

            // Use this if you are reading messages from EventHub directly.
            m_SbConnStr = ConfigurationManager.AppSettings["ServiceBus.ServiceConnStr"];
            m_EventHubPath = ConfigurationManager.AppSettings["ServiceBus.EventHubPath"];

            Console.WriteLine("Receive messages\n");

            eventHubClient = EventHubClient.CreateFromConnectionString(m_SbConnStr, m_EventHubPath);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                ReceiveMessagesFromDeviceAsync(partition);
            }

            Console.ReadLine();
        }

        private async static Task ReceiveMessagesFromDeviceAsync(string partition)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.Now);
            while (true)
            {
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());

                dynamic obj = JsonConvert.DeserializeObject(data);
                if (obj.sensor == "Accelerometer")
                    writeAcceleretorDataToFile(obj);
                else if (obj.sensor == "HeartRate")
                {

                }
                else
                {

                }

                Console.WriteLine(string.Format("Message received. Partition: {0} Data: '{1}'", partition, data));
            }
        }

        private static void deserializeMessage(string data)
        {
            dynamic obj = JsonConvert.DeserializeObject(data);

        }

        private static DateTime m_LastEvent = new DateTime();

        private static StreamWriter m_Stream;

        private static void writeAcceleretorDataToFile(dynamic obj)
        {
            string line = String.Format("{0},{1},{2},{3}", obj.x, obj.y, obj.z, obj.ts);

            DateTimeOffset eTime = (DateTimeOffset)obj.ts;
            DateTime now = DateTime.Now;

            return;

            lock (":)")
            {
                if (m_Stream == null)
                {
                    createFile(obj, now);
                }
                else
                {
                    if ((now - m_LastEvent) > TimeSpan.FromMinutes(1))
                    {
                        m_LastEvent = now;

                        if (m_Stream != null)
                        {
                            m_Stream.Flush();
                            m_Stream.Close();
                        }

                        createFile(obj, now);
                    }
                }
            }

            m_Stream.WriteLine(line);

            m_Stream.Flush();
        }

        private static void createFile(dynamic obj, DateTime now)
        {
            string suffix = String.Format("{0}{1}{2}{3}{4}", now.Year, now.Month, now.Day, now.Hour, now.Minute);
            string fileName = String.Format("{0}_{1}.csv", obj.scenario.Value as string, suffix);
            m_Stream = new StreamWriter(fileName);
        }
    }
}
