using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace CosmosPerfTests
{
    public static class Credentials
    {
        public static class MongoDb
        {

            public const string ConnectionString = @"mongodb://daenetcosmos:I4pxJURbOxdtaWe8abcicqqDRnMNfZob0mhNS8HNT37zDdlvGOVHdGmemuXWeSYCS2MDtOno9l3brKMMrnDeJQ==@daenetcosmos.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";
            public const string UserName = "";
            public const string Host = "";
            public const string Password = "";
        }

        public static class DocumentDb
        {
            public const string EndpointUri = "https://servicebook-dev.documents.azure.com:443/";
            public const string Key = "";
            public const string DatabaseName = "ToDoList";
            public const string CollectionName = "Items";
        }
    }
}
