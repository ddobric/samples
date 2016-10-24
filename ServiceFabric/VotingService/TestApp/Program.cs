using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //setupMetrics();
            //setupService();

            traceAll();
          
            ////return;
            //CancellationTokenSource src = new CancellationTokenSource();
            //var resolver = ServicePartitionResolver.GetDefault();
           
            //var partKey = new ServicePartitionKey("262887c1-d67c-41da-a32f-0db2e8cf0e96");

            //resolver.ResolveAsync(new Uri("fabric:/VotingSvcSolution/VotingService"), 
            //   partKey,
            //    src.Token).Wait();

        }


        private static void setupMetrics()
        {
            FabricClient fabricClient = new FabricClient();

            ServiceLoadMetricDescription metric = new StatefulServiceLoadMetricDescription();
            metric.Name = "Memory";
            metric.PrimaryDefaultLoad = 1024;
            metric.SecondaryDefaultLoad = 1024;
            metric.Weight = ServiceLoadMetricWeight.High;
         
            StatefulServiceUpdateDescription updateDescription = new StatefulServiceUpdateDescription();
          //  updateDescription.Metrics = new KeyedCollection<string, StatefulServiceLoadMetricDescription>();

            updateDescription.Metrics.Add(metric);

            fabricClient.ServiceManager.UpdateServiceAsync(new Uri("fabric:/App3/App3Svc1"), updateDescription).Wait();
  
        }
        private static void setupService()
        {
            FabricClient fabricClient = new FabricClient();
           
            var health = fabricClient.HealthManager.GetApplicationHealthAsync(new Uri("fabric:/VotingSvcSolution"), 
                new System.Fabric.Health.ApplicationHealthPolicy()).Result;

            var appList = fabricClient.QueryManager.GetApplicationListAsync().Result;
            
            var serviceDescription = fabricClient.ServiceManager.GetServiceDescriptionAsync(new Uri("fabric:/VotingSvcSolution/VotingService")).Result;

        
            // add other required servicedescription fields
            //...
            fabricClient.ServiceManager.CreateServiceAsync(serviceDescription).Wait();

            StatefulServiceUpdateDescription updateDescription = new StatefulServiceUpdateDescription();
            updateDescription.PlacementConstraints = "NodeType == NodeType01";
            fabricClient.ServiceManager.UpdateServiceAsync(new Uri("fabric:/app/service"), updateDescription);
        }

        private static void getPartitionUrlTest()
        {
            while (true)
            {
                ServicePartitionInformation inf;

                var url = getPartitionUrl(0, out inf);

                Console.WriteLine(url);

                Thread.Sleep(2500);
            }
        }


        ///
        private static void traceAll()
        {
            /*
              Note that APplicationManifest.Xml must have UniformPartition type with keys 0-10!

              <StatefulService ServiceTypeName="VotingServiceType" TargetReplicaSetSize="[VotingService_TargetReplicaSetSize]" MinReplicaSetSize="[VotingService_MinReplicaSetSize]">
                    <UniformInt64Partition PartitionCount="[VotingService_PartitionCount]" LowKey="0" HighKey="100" />           
                </StatefulService> 
            */

            for (int i = 1; i < 6; i++)
            {
                ServicePartitionInformation inf;

                var url = getPartitionUrl(i, out inf);

                Console.WriteLine($"paritionKey:{i}-\tpartitionId={inf.Id}\tUrl:{url}");
            }
        }

        private static string getPartitionUrl(long partitionKey, out ServicePartitionInformation info)
        {
            var partKey = new ServicePartitionKey(partitionKey);

            return getPartitionUrl(partKey, out info);
        }

        private static string getPartitionUrl(ServicePartitionKey partitionKey, out ServicePartitionInformation info)
        {
            CancellationTokenSource src = new CancellationTokenSource();

            var resolver = ServicePartitionResolver.GetDefault();

            var partition = resolver.ResolveAsync(new Uri("fabric:/VotingSvcSolution/VotingService"), partitionKey, src.Token).Result;

            var pEndpoint = partition.GetEndpoint();

            var primaryEndpoint = partition.Endpoints.FirstOrDefault(p => p.Role == System.Fabric.ServiceEndpointRole.StatefulPrimary);
            info = partition.Info;
            if (primaryEndpoint != null)
            {
                JObject addresses = JObject.Parse(primaryEndpoint.Address);

                var p = addresses["Endpoints"].First();

                string primaryReplicaAddress = p.First().Value<string>();

                return primaryReplicaAddress;
            }
            else
                return ":(";
        }
    }
}
