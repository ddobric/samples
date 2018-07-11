using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace eventgridpublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine($"Application required 4 arguments. key, topicHostName, eventDataFile, eventType.");
                Console.WriteLine($"Example: eventgridpublisher 'm9Nc1uLYS38gX72Cdj/P6Dr4evqjmMQuwXi5NyDuTPY=' 'https://treining-eventgrid-topic.westeurope-1.eventgrid.azure.net/api/events' sampleevents event1.json 'eventgrid/training/event1' ");
                Console.WriteLine($"Example: eventgridpublisher 'm9Nc1uLYS38gX72Cdj/P6Dr4evqjmMQuwXi5NyDuTPY=' 'https://treining-eventgrid-topic.westeurope-1.eventgrid.azure.net/api/events' sampleevents *.json 'eventgrid/training/event1' ");
                return;
            }

            sendEvent(args[0].Replace("'", String.Empty), new Uri(args[1].Replace("'", String.Empty)).Host, args[2].Replace("'", String.Empty), args[3].Replace("'", String.Empty), args[4].Replace("'", String.Empty)).Wait();
        }

        private static async Task sendEvent(string key, string topicHostName, string eventFileFolder, string eventFileFilter, string eventType)
        {
            List<string> eventData = new List<string>();
            
            foreach (var file in Directory.GetFiles(eventFileFolder, eventFileFilter))
            {
                using (StreamReader sw = new StreamReader(file))
                {
                    eventData.Add(sw.ReadToEnd());
                }
            }

            ServiceClientCredentials credentials = new TopicCredentials(key);
            var client = new EventGridClient(credentials);

            var events = new List<EventGridEvent>
            {
                new EventGridEvent()
                {
                    Id = Guid.NewGuid().ToString(),
                    Data = eventData,
                    EventTime = DateTime.Now,
                    EventType = eventType,
                    Subject = eventType,
                    DataVersion = "1.0"
                }
            };

         
            await client.PublishEventsAsync(topicHostName, events);
        }
    }
}
