using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace CosmosPerfTests
{
    public class MongoSample : ISample<TelemetryMongo> , IDisposable
    {
        private bool disposed = false;
        
        private string dbName = "perf-test-dbid";

        private string collectionName = "TelemetryData";      

        // Gets all Task items from the MongoDB server.        
        public async Task<List<TelemetryMongo>> GetAllTelemetryData()
        {
            try
            {
                var collection = GetTasksCollection();

                FilterDefinition<TelemetryMongo> filter = FilterDefinition<TelemetryMongo>.Empty;
                
                FindOptions<TelemetryMongo> options = new FindOptions<TelemetryMongo>
                {
                    BatchSize = 2,
                    NoCursorTimeout = false
                };

                var data = await collection.FindAsync(filter, options);

                return await data.ToListAsync();
                //while (await data.MoveNextAsync())
                //{
                //    IEnumerable<Telemetry> batch = data.Current;
                //    foreach (Telemetry document in batch)
                //    {
                //        Console.WriteLine(document);
                //        Console.WriteLine();
                //    }
                //}

             
            }
            catch (MongoConnectionException)
            {
                return new List<TelemetryMongo>();
            }
        }

        public async Task DeleteRecordAsync(TelemetryMongo record)
        {
            var collection = GetTasksCollection();

            FilterDefinition<TelemetryMongo> filter = FilterDefinition<TelemetryMongo>.Empty;

            FindOptions<TelemetryMongo> options = new FindOptions<TelemetryMongo>
            {
                BatchSize = 2,
                NoCursorTimeout = false
            };

            await collection.DeleteOneAsync<TelemetryMongo>((rec) => rec._id == record._id);
        }


        public async Task DeleteRecordAsync(TelemetryMongo[] records)
        {
            var collection = GetTasksCollection();

            FilterDefinition<TelemetryMongo> filter = FilterDefinition<TelemetryMongo>.Empty;

            FindOptions<TelemetryMongo> options = new FindOptions<TelemetryMongo>
            {
                BatchSize = 2,
                NoCursorTimeout = false
            };

            await collection.DeleteManyAsync<TelemetryMongo>((rec) => true);
        }

        // Creates a Task and inserts it into the collection in MongoDB.
        public async Task SaveTelemetryData(TelemetryMongo task)
        {
            var collection = GetTasksCollection();
            try
            {
                await collection.InsertOneAsync(task);
            }
            catch (MongoCommandException ex)
            {
                string msg = ex.Message;
            }
        }


        public async Task SaveTelemetryData(TelemetryMongo[] tasks)
        {
            var collection = GetTasksCollection();
            try
            {
                await collection.InsertManyAsync(tasks);
            }
            catch (MongoCommandException ex)
            {
                string msg = ex.Message;
            }
        }


        private IMongoCollection<TelemetryMongo> GetTasksCollection()
        {           
            MongoClient client = new MongoClient(getSettings());
            var database = client.GetDatabase(dbName);
            var todoTaskCollection = database.GetCollection<TelemetryMongo>(collectionName);
            return todoTaskCollection;
        }

        //private IMongoCollection<Telemetry> GetTasksCollectionForEdit()
        //{
        //    MongoClient client = new MongoClient(getSettings());
        //    var database = client.GetDatabase(dbName);
        //    var todoTaskCollection = database.GetCollection<Telemetry>(collectionName);
        //    return todoTaskCollection;
        //}

        private MongoClientSettings getSettings()
        {
            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress(Credentials.MongoDb.Host, 10255);
            settings.UseSsl = true;
            settings.SslSettings = new SslSettings();
            settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;

            MongoIdentity identity = new MongoInternalIdentity(dbName, Credentials.MongoDb.UserName);
            MongoIdentityEvidence evidence = new PasswordEvidence(Credentials.MongoDb.Password);

            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);

            return settings;
        }
        # region IDisposable

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }
            }

            this.disposed = true;
        }

     



        #endregion
    }
}