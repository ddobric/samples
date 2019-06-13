using CosmosTableSamples.Model;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace CosmosTableSamples
{
    public class ZfSamples
    {
        /// <summary>
        /// ZF
        /// </summary>
        /// <param name="table"></param>
        /// <param name="telemetryEvents"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public static async Task BatchInsertOfTelemetryAsync(CloudTable table, List<TelemetryEventEntity> telemetryEvents)
        {
            try
            {
                // Create the batch operation. 
                TableBatchOperation batchOperation = new TableBatchOperation();

                foreach (var msg in telemetryEvents)
                {
                    batchOperation.InsertOrMerge(msg);
                }

                // Execute the batch operation.
                TableBatchResult results = await table.ExecuteBatchAsync(batchOperation);
                foreach (var res in results)
                {
                    var customerInserted = res.Result as TelemetryEventEntity;
                    Console.WriteLine("Inserted entity with\t Etag = {0} and PartitionKey = {1}, RowKey = {2}", customerInserted.ETag, customerInserted.PartitionKey, customerInserted.RowKey);
                }

                if (results.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of the Batch Operation against Cosmos DB Table: " + results.RequestCharge);
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public static void ZfExecuteCarQuery(CloudTable table, string carId, string startRowKey,
          string endRowKey)
        {
            try
            {
                // Create the range query using the fluid API 
                TableQuery<TelemetryEventEntity> rangeQuery = new TableQuery<TelemetryEventEntity>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, carId),
                        TableOperators.And,
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual,
                                startRowKey),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual,
                                endRowKey))));

                Stopwatch sw = new Stopwatch();
                sw.Start();
                var lst = table.ExecuteQuery(rangeQuery).ToList();
                sw.Stop();
                Console.WriteLine($"Returned: {lst.Count}. Time: {sw.ElapsedMilliseconds}");

                foreach (TelemetryEventEntity entity in table.ExecuteQuery(rangeQuery))
                {
                    Console.WriteLine("Customer: {0},{1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.Email,
                        entity.PhoneNumber);
                }
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }


        public static async Task<List<string>> ZfPopulateTelemetryEventsAsync(CloudTable table)
        {
            List<string> carIdList = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                carIdList.Add(Guid.NewGuid().ToString());
            }

            int batchCnt = 0;
            int batchSize = 99;

            List<TelemetryEventEntity> batch = new List<TelemetryEventEntity>();

            for (int car = 0; car < carIdList.Count; car++)
            {
                for (int i = 0; i < 100000000; i++)
                {
                    TelemetryEventEntity msg = new TelemetryEventEntity(carIdList[car], DateTime.Now.ToString("o"), Guid.NewGuid().ToString());

                    batch.Add(msg);

                    if (batchCnt++ == batchSize)
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        
                        await ZfSamples.BatchInsertOfTelemetryAsync(table, batch);
                        batch.Clear();

                        sw.Stop();
                        Console.WriteLine($"Data imported {i}, time: {sw.ElapsedMilliseconds}");
                        batchCnt = 0;
                    }
                }
            }

            Console.WriteLine();

            return carIdList;
        }

    }
}
