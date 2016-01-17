

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System.Configuration;

namespace GuestBook_Data
{
    public class TblStorageSamples
    {
        #region Credentials
        private static string m_AccKey = "";
        private static string m_AccName = "";
        #endregion

        private static CloudStorageAccount m_StorageAccount;

        private static string m_TblName = "TGuestBook";

      
        static TblStorageSamples()
        {
            m_AccKey = ConfigurationManager.AppSettings["AccountKey"];
            m_AccName = ConfigurationManager.AppSettings["AccountName"];

            m_StorageAccount = getAccount();

            CloudTableClient tableClient = new CloudTableClient(m_StorageAccount.TableEndpoint,
                m_StorageAccount.Credentials);

            
            var table = tableClient.GetTableReference(m_TblName);

            table.CreateIfNotExists();
        }

        public void DeleteTables(string strtsWith)
        {
            CloudTableClient tableClient = new CloudTableClient(m_StorageAccount.TableEndpoint,
                 m_StorageAccount.Credentials);

            var tables = tableClient.ListTables(strtsWith);
            foreach(var tbl in tables)
            {           
                tbl.Delete();                
            }
        }

        private static CloudTable getTableClient()
        {
            CloudTableClient tableClient = new CloudTableClient(m_StorageAccount.TableEndpoint,
               m_StorageAccount.Credentials);

            return tableClient.GetTableReference(m_TblName);
        }

        private static CloudStorageAccount getAccount()
        {

            CloudStorageAccount storageAccount =
                new CloudStorageAccount(new StorageCredentials(m_AccName, m_AccKey), true);


            //CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            //var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");

            return storageAccount;
        }

        //public GuestBookEntryDataSource()
        //{
        //    this.m_Context = new GuestBookDataContext(m_StorageAccount.TableEndpoint.AbsoluteUri,
        //        m_StorageAccount.Credentials);
        //   // this.m_Context.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(1));
        //}

        public IEnumerable<GuestBookEntry> SelectAll()
        {
            var table = getTableClient();

            TableQuery<GuestBookEntry> query = new TableQuery<GuestBookEntry>();
      
            var results = from g in table.CreateQuery<GuestBookEntry>()
                          where g.PartitionKey != ""
                          select g;

            return results;
     
        }

        public IEnumerable<GuestBookEntry> QueryByName(string name)
        {
            var table = getTableClient();

           // TableQuery<GuestBookEntry> query = new TableQuery<GuestBookEntry>();

            var results = from g in table.CreateQuery<GuestBookEntry>()
                          where g.GuestName == name
                          select g;

            return results;
        }

        public IEnumerable<GuestBookEntry> QueryByMyProperty(int val)
        {
            var table = getTableClient();

            TableQuery<GuestBookEntry> query = new TableQuery<GuestBookEntry>();

            var results = from g in table.CreateQuery<GuestBookEntry>()
                          where g.MyProperty == val
                          select g;
           
            return results;           
        }


        public void DeleteEntriesSequentially(ICollection<GuestBookEntry> deletingIds)
        {
            var table = getTableClient();

            TableBatchOperation batch = new TableBatchOperation();

            foreach (var book in deletingIds)
            {
                book.ETag = "*";
                TableOperation deleteOperation = TableOperation.Delete(book);
                table.Execute(deleteOperation);
            }
        }

        public void DeleteEntriesAsBatch(ICollection<GuestBookEntry> deletingIds)
        {
            var table = getTableClient();

            TableBatchOperation batch = new TableBatchOperation();

            if (deletingIds.Count > 0)
            {
                foreach (var book in deletingIds)
                {
                    book.ETag = "*";
                    TableOperation deleteOperation = TableOperation.Delete(book);
                    batch.Add(deleteOperation);
                }

                table.ExecuteBatch(batch);
            }
        }


        public void UpdateGuestBookEntry(GuestBookEntry existingItem)
        {
            var table = getTableClient();

            TableBatchOperation batch = new TableBatchOperation();

            TableOperation updateOperation = TableOperation.Merge(existingItem);             

            table.Execute(updateOperation);
        }


        public void AddGuestBookEntries(List<GuestBookEntry> books)
        {
            var table = getTableClient();

            TableBatchOperation batch = new TableBatchOperation();

            foreach (var book in books)
            {
                TableOperation insertOperation = TableOperation.Insert(book);
                batch.Add(insertOperation);
            }

            table.ExecuteBatch(batch);
        }

        public void UpdateImageThumbnail(string partitionKey, string rowKey, string thumbUrl)
        {
            //var results = from g in this.m_Context.GuestBookEntry
            //              where g.PartitionKey == partitionKey && g.RowKey == rowKey
            //              select g;

            //var entry = results.FirstOrDefault<GuestBookEntry>();
            //entry.ThumbnailUrl = thumbUrl;
            //this.m_Context.UpdateObject(entry);
            //this.m_Context.SaveChanges();
        }
    }
}
