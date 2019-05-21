using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Linq;

namespace configservice
{
    // See: https://docs.microsoft.com/en-us/azure/azure-app-configuration/
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("ConnectionString"));

            var config = builder.Build();
            Console.WriteLine(config["TestApp:Settings:Message"] ?? "Hello world!");
        }
    }
}
