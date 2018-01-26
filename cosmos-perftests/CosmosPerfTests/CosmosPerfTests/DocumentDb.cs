using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Performance tips:
// https://stackoverflow.com/questions/41744582/fastest-way-to-insert-100-000-records-into-documentdb

namespace CosmosPerfTests
{
    public class DocumentDb : ISample<TelemetryDocDb>, IDisposable
    {

        public async Task DeleteRecordAsync(TelemetryDocDb record)
        {
            var client = getClient();

            var docUri = UriFactory.CreateDocumentUri(Credentials.DocumentDb.DatabaseName, Credentials.DocumentDb.CollectionName, record.id);

            var book = await client.DeleteDocumentAsync(docUri/*, new RequestOptions { PartitionKey = new PartitionKey(1) }*/);
        }

        public async Task DeleteRecordAsync(TelemetryDocDb[] telemetryData)
        {
            var client = getClient();

            foreach (var doc in telemetryData)
            {             
                var docUri = UriFactory.CreateDocumentUri(Credentials.DocumentDb.DatabaseName, Credentials.DocumentDb.CollectionName, doc.id);

                var book = await client.DeleteDocumentAsync(docUri/*, new RequestOptions { PartitionKey = new PartitionKey(1) }*/);

            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task<List<TelemetryDocDb>> GetAllTelemetryData()
        {
            return Task<List<TelemetryDocDb>>.Run(() =>
            {
                var client = getClient();

                FeedOptions queryOptions = new FeedOptions { MaxItemCount = 500, EnableCrossPartitionQuery = true };

                IQueryable<TelemetryDocDb> books = client.CreateDocumentQuery<TelemetryDocDb>(
                   UriFactory.CreateDocumentCollectionUri(Credentials.DocumentDb.DatabaseName, Credentials.DocumentDb.CollectionName),
                   $"SELECT * FROM c",
                   queryOptions);

                return books.ToList();
            });

        }

        public async Task SaveTelemetryData(TelemetryDocDb telemetryEvent)
        {
            var client = getClient();
            var res = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Credentials.DocumentDb.DatabaseName, Credentials.DocumentDb.CollectionName), telemetryEvent);
        }

        public async Task SaveTelemetryData(TelemetryDocDb[] events)
        {
            var client = getClient();
            foreach (var telemetryEvent in events)
            {
                var res = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Credentials.DocumentDb.DatabaseName, Credentials.DocumentDb.CollectionName), telemetryEvent);
            }
        }

        private bool disposed;

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

        private DocumentClient getClient()
        {
            DocumentClient client = new DocumentClient(new Uri(Credentials.DocumentDb.EndpointUri),
                Credentials.DocumentDb.Key);

            return client;
        }
    }
}
