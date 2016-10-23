using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

namespace Daenet.WinAzure.QueuesLab
{
    class Program
    {
        #region Credentials             
        private const string m_AccKey = "iVDxtngI2PYqQG+/9+tr6saIZ+l5JxCacRxK1AzmQI7XyHssvH2e7qfXx6CZSTHtJ7pYMLzY3PxZ627zvLiIAw==";
        private const string m_AccName = "daenet01";
        #endregion      

        static void Main(string[] args)
        {
            createQueues();

            listQueues();

            sendMessage();

            getMessage();

            deletMessages();


        }

        private static void createQueues()
        {
            CloudQueueClient client = getAccount().CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference("queue1");
            if (!queue.Exists())
            {
                queue.Create();
            }

       

            CloudQueue queue2 = client.GetQueueReference("queue3");
            if (!queue2.Exists())
            {
                queue2.Create();
            }

           
        }


        private static CloudStorageAccount getAccount()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentialsAccountAndKey(m_AccName, m_AccKey), true);
            storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            return storageAccount;
        }


        private static void listQueues()
        {
            List<CloudQueue> queueList = new List<CloudQueue>();

            CloudQueueClient client = getAccount().CreateCloudQueueClient();
            IEnumerable<CloudQueue> queues = client.ListQueues();
            if (queues != null)
            {
                foreach (var q in queues)
                {
                    Console.WriteLine(q.Name);
                }
            }
        }

        private static void sendMessage()
        {
            CloudQueueClient client = getAccount().CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference("queue1");
            queue.AddMessage(new CloudQueueMessage("Message 1"));
            queue.AddMessage(new CloudQueueMessage("Message 2"));
        }

        private static void getMessage()
        {
            CloudQueueClient client = getAccount().CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference("queue1");
            var msg = queue.GetMessage(TimeSpan.FromMinutes(1));
            Console.WriteLine(msg.AsString);

            msg.SetMessageContent(msg.AsString + " Updated contents.");
            queue.UpdateMessage(msg,
                TimeSpan.FromSeconds(0.0),  // visible immediately
                MessageUpdateFields.Content | MessageUpdateFields.Visibility);

            msg = queue.GetMessage(TimeSpan.FromMinutes(1));
            Console.WriteLine(msg.AsString);
            

            //queue.DeleteMessage(msg);


            //msg = queue.GetMessage(TimeSpan.FromMinutes(1));
            //Console.WriteLine(msg.AsString);
        }

        private static void deletMessages()
        {
            List<CloudQueue> queueList = new List<CloudQueue>();

            CloudQueueClient client = getAccount().CreateCloudQueueClient();
            IEnumerable<CloudQueue> queues = client.ListQueues();
            if (queues != null)
            {
                foreach (var q in queues)
                {
                    q.Clear();
                }
            }
        }
    }
}
