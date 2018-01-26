using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CosmosPerfTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //ISample<TelemetryMongo> sample = new MongoSample();
            ISample<TelemetryDocDb> sample = new DocumentDb();

            Console.WriteLine($"Test started...");

            RunAsync(sample).Wait();

            Console.WriteLine($"Test completed. Press any key to exit.");

            Console.ReadLine();

        }

        private static async Task RunAsync<T>(ISample<T> sample) where T : class
        {
          
            int batchSize = 20;

            //
            // Adding single records
            //

            Stopwatch watch = new Stopwatch();

            Console.WriteLine($"Uploading {batchSize} records ony by one.");

            watch.Start();

            for (int i = 0; i < batchSize; i++)
            {
                var data = getSampleData<T>();

                await sample.SaveTelemetryData(data);
            }

            watch.Stop();

            Console.WriteLine($"Completed with: {watch.ElapsedMilliseconds}");

            Console.WriteLine($"Getting {batchSize} records");

            watch.Restart();

            //
            // Get all data from store.
            //

            var telemetryData = await sample.GetAllTelemetryData();

            watch.Stop();

            Console.WriteLine($"All records returned in: {watch.ElapsedMilliseconds}");


            //
            // Deleting single record
            //

            Console.WriteLine($"Deleting {batchSize} records one by one");

            watch.Restart();

            foreach (var record in telemetryData)
            {
                Console.Write(".");

                await sample.DeleteRecordAsync(record);
            }

            watch.Stop();

            Console.WriteLine();

            Console.WriteLine($"All records deleted in: {watch.ElapsedMilliseconds}");


            //
            // Adding batch of records
            //

            Console.WriteLine($"Uploading {batchSize} records");

            watch.Restart();
            
            List<T> telList = new List<T>();

            for (int i = 0; i < batchSize; i++)
            {
                telList.Add(getSampleData<T>());
            }

            await sample.SaveTelemetryData(telList.ToArray());

            watch.Stop();

            Console.WriteLine($"Record batch added in: {watch.ElapsedMilliseconds}");

            Console.WriteLine($"Start returning {batchSize} records.");

            watch.Restart();

            //
            // Deleting of all records
            //
            telemetryData = await sample.GetAllTelemetryData();

            Console.WriteLine($"Returned batch of {batchSize} records in: {watch.ElapsedMilliseconds}");

            Console.WriteLine($"Deleting {batchSize} records at once.");

            watch.Restart();

            await sample.DeleteRecordAsync(telemetryData.ToArray());


            watch.Stop();

            Console.WriteLine($"All records deleted in: {watch.ElapsedMilliseconds}");

            
        }

        private static Random m_Random = new Random();

        private static T getSampleData<T>()
        {
            if (typeof(T) == typeof(TelemetryMongo))
            {
                return (T)(object)new TelemetryMongo()
                {
                    DeviceId = $"DEV_{m_Random.Next(1, 1000)}",

                    Temperature = m_Random.Next(25, 35) + m_Random.NextDouble(),
                };
            }
            else if (typeof(T) == typeof(TelemetryDocDb))
            {
                return (T)(object)new TelemetryDocDb()
                {
                    DeviceId = $"DEV_{m_Random.Next(1, 1000)}",

                    Temperature = m_Random.Next(25, 35) + m_Random.NextDouble(),
                };
            }
            else
                throw new Exception();
        }
    }
}
