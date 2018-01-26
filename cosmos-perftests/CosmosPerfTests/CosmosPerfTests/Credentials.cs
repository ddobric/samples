using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace CosmosPerfTests
{
    public static class Credentials
    {
        public static class MongoDb {

           public const string connectionString = "mongodb://";
           public const string UserName = "TODO";
           public const string Host = "TODO";
           public const string Password = "TODO";
        }

        public static class DocumentDb
        {
            public const string EndpointUri = "TODO";
            public const string Key = "TODO";
            public const string DatabaseName = "TODO";
            public const string CollectionName = "TODO";
        }
    }
}
