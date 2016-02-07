using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    class Program
    {
        static ServiceClient m_ServiceClient;

    
        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");

            string connStr = ConfigurationManager.AppSettings["IotHub.ServiceConnStr"];

            m_ServiceClient = ServiceClient.CreateFromConnectionString(connStr);
            
            Console.WriteLine("Press any key to send a C2D message.");
            Console.ReadLine();

            SendCloudToDeviceMessageAsync().Wait();

            Console.ReadLine();
        }

        private async static Task SendCloudToDeviceMessageAsync()
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("STOP")) ;
            commandMessage.Ack = DeliveryAcknowledgement.Full;

            string deviceId = ConfigurationManager.AppSettings["IotHub.DeviceId"];

            await m_ServiceClient.SendAsync(deviceId, commandMessage);

            Start_ReceivingFeedback();
        }

        private async static void Start_ReceivingFeedback()
        {
            var feedbackReceiver = m_ServiceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d feedback from service");

            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Device feedback: {0}", string.Join(", ", 
                    feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
    }
}
