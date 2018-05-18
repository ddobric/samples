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

            public const string ConnectionString = @"";
            public const string UserName = "";
            public const string Host = "";
            public const string Password = "";
        }

        public static class DocumentDb
        {
            public const string EndpointUri = "";
            public const string Key = "";
            public const string DatabaseName = "ToDoList";
            public const string CollectionName = "Items";
        }
    }
}
