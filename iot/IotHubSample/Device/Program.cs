using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Device
{
    class Program
    {
        static DeviceClient m_DeviceClient;
     
        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device started...\n");

            string connStr = ConfigurationManager.AppSettings["IotHub.DeviceConnStr"];
            string deviceId = ConfigurationManager.AppSettings["IotHub.DeviceId"];

            m_DeviceClient = DeviceClient.CreateFromConnectionString(connStr, deviceId,);

            //Start_SendingMessagesToCloud();

            Start_ReceivingMessages();

            Console.ReadLine();
        }


        private static async void Start_SendingMessagesToCloud()
        {
            string deviceId = ConfigurationManager.AppSettings["IotHub.DeviceId"];
            Random rand = new Random();
            List<string> geoData = new List<string>();
            geoData.Add("Frankfurt am Main");// 50.1109901,8.6828398");
            geoData.Add("Seattle");// 47.6147628,-122.475987");

            while (true)
            {
                double strom = rand.Next(1, 5) + rand.NextDouble();
                double temperature = rand.Next(30, 70);
                var indx = rand.Next(0, 2);

                var telemetryDataPoint = new
                {
                    deviceId = deviceId,
                    Temperature = temperature,
                    Current = strom,
                    Location = geoData[indx],
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);

                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await m_DeviceClient.SendEventAsync(message);
             
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Thread.Sleep(5000);
            }
        }


        private static async void Start_ReceivingMessages()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await m_DeviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

               // await m_DeviceClient.AbandonAsync(receivedMessage);
                await m_DeviceClient.CompleteAsync(receivedMessage);
            }
        }
    }
}
