using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using StreamReaderFunctionApp;

namespace StreamFuncApp
{
    public static class Function1
    {
        private static CloudStorageAccount storageAccount;

        private static string connStr = "DefaultEndpointsProtocol=https;AccountName=openhackiotdata010stor;AccountKey=UOAQQj2ZVojL6yoyRFD8DySVkA7Y6y9dXx6ADjS6nBYC02ospofrKwlUwP26BL5TbYJWc1SwWx85cCV+3NeT/g==;EndpointSuffix=core.windows.net";

        private static CloudBlobClient client;

        private static CloudBlobContainer container;


        static Function1()
        {
            storageAccount = CloudStorageAccount.Parse(connStr);

            client = storageAccount.CreateCloudBlobClient();

            container = client.GetContainerReference("funcdata");

            //container.CreateIfNotExistsAsync().Wait();
        }

        [FunctionName("GpsReaderFunc1")]
        //public static void Run([EventHubTrigger("t10iothub", Connection = "iothubconnstr",
        //    ConsumerGroup = "Function_ProcessBleBasePackets")]GpsMsg[] iotHubMessages, TraceWriter logger)
        public static async Task Run1([EventHubTrigger("t10iothub", Connection = "iothubconnstr",
            ConsumerGroup = "FunctionApp")]TicketMsg[] iotHubMessages, TraceWriter logger)
        {
            logger.Info($"{iotHubMessages}");
            Console.WriteLine($"{iotHubMessages}");
            foreach (var item in iotHubMessages)
            {
                if (item.ticketId == null) continue;

                await writeToBlob("TicketMsg", item.ticketId, item.entryTime, logger);
            }
        }


        [FunctionName("GpsReaderFunc2")]
        //public static void Run([EventHubTrigger("t10iothub", Connection = "iothubconnstr",
        //    ConsumerGroup = "Function_ProcessBleBasePackets")]GpsMsg[] iotHubMessages, TraceWriter logger)
        public static async Task Run2([EventHubTrigger("t10iothub", Connection = "iothubconnstr",
            ConsumerGroup = "FunctionApp")]PositionMsg[] iotHubMessages, TraceWriter logger)
        {
            logger.Info($"{iotHubMessages}");
            Console.WriteLine($"{iotHubMessages}");
            foreach (var item in iotHubMessages)
            {
                if (item.rideId == null) continue;

                await writeToBlob("PositionMsg", item.correlationId, item.deviceTime, logger);
            }
        }

        private static async Task writeToBlob(string msgType, string correlationId, string timestamp, TraceWriter logger)
        {
            //var m = JsonConvert.DeserializeObject<PositionMsg>(iotHubMessages);
            logger.Info($"{timestamp}");
            logger.Info($"{timestamp}");
            var blob = container.GetBlockBlobReference($"{msgType}-{Guid.NewGuid().ToString("n")} - {timestamp}");

            await blob.UploadTextAsync(timestamp);
        }
    }
}
