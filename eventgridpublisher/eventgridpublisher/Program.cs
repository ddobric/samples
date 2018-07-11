using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace eventgridpublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            AssemblyInformationalVersionAttribute version = Assembly
            .GetEntryAssembly()?
            .GetCustomAttribute(typeof(System.Reflection.AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
           
            Console.WriteLine($"Event Grid Publisher version {version.InformationalVersion}");

            if (args.Length != 5)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Application requires 5 arguments: key, topicHostName, eventFileFolder eventFileFilter, eventType.");
                Console.WriteLine($"Example: eventgridpublisher '***' 'https://***.westeurope-1.eventgrid.azure.net/api/events' sampleevents event1.json 'eventgrid/training/event1' ");
                Console.WriteLine($"Example: eventgridpublisher '***' 'https://***.westeurope-1.eventgrid.azure.net/api/events' sampleevents *.json 'eventgrid/training/event1' ");
                Console.ResetColor();
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

            if (eventData.Count > 0)
            {
                Console.WriteLine($"Sending {eventData.Count} events.");

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

                int i = 0;
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (var item in eventData)
                {
                    i++;
                    Console.WriteLine($" ---------------- Event {i} ----------------");

                    Console.WriteLine(item);

                    Console.WriteLine("---------------------------------------------");
                }

                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("Events sent successfully.");
            }
            else
                Console.WriteLine("No event file has been found.");
          
        }
    }
}
