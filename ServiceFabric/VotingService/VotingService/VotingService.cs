using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VotingService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class VotingService : StatefulService
    {
        public VotingService(StatefulServiceContext context)
            : base(context)
        { }


        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            //   "Message": "Tcp://+:20002/907db591-b0b1-4342-b91b-d63e6350ae64/131216327628253690-bc15dfce-5cf4-4d60-ae79-a7d98670d934/",

            EndpointResourceDescription internalEndpoint = Context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            var uriPrefix = $"https://+:{internalEndpoint.Port}/"
               + $"{Context.PartitionId}/{Context.ReplicaId}"
               + $"-{Guid.NewGuid()}/";   // Uniqueness

            ServiceEventSource.Current.ServiceMessage(this, uriPrefix);

            Debug.WriteLine(uriPrefix);

            return new ServiceReplicaListener[]
         { new ServiceReplicaListener(p => new HttpCommunicationListener(p, "ServiceEndpoint", processRequest)) };
        }


        private async Task processRequest(HttpListenerContext context, CancellationToken cancelationToken)
        {
            String output = null;
            try
            {
                // Grab the vote item string from a "Vote=" query string parameter
                HttpListenerRequest request = context.Request;
                String voteItem = request.QueryString["Vote"];

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var voteDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

                    if (voteItem != null)
                    {                      
                        var keyPair = await voteDictionary.TryGetValueAsync(tx, voteItem, LockMode.Default);
                        if (!keyPair.HasValue)
                        {
                            await voteDictionary.AddAsync(tx, voteItem, 1);
                        }
                        else
                        {
                            var res = await voteDictionary.TryUpdateAsync(tx, voteItem, keyPair.Value + 1, keyPair.Value);
                        }                        
                    }
                  
                    // Old SDK version!
                    // The code below prepares the HTML response. It gets all the current
                    // vote items (and counts) and separates each with a break (<br>)
                    //var votingItems = from kvp in await voteDictionary.CreateEnumerableAsync(tx)
                    //                  orderby kvp.Key    // Intentionally commented out
                    //                  select $"Item={kvp.Key}, Votes={kvp.Value}";

                    var votingItems = await voteDictionary.CreateEnumerableAsync(tx);
                    var enumerator = votingItems.GetAsyncEnumerator();

                    StringBuilder sb = new StringBuilder();

                    while (await enumerator.MoveNextAsync(cancelationToken))
                    {
                        sb.Append($"{enumerator.Current.Key} - Votes: {enumerator.Current.Value}<br/>");
                    }

                    output = sb.ToString();

                    await tx.CommitAsync();
                }


            }
            catch (Exception ex)
            {
                output = ex.ToString();
            }

            // Write response to client:
            using (var response = context.Response)
            {
                if (output != null)
                {
                    Byte[] outBytes = Encoding.UTF8.GetBytes(output);
                    response.OutputStream.Write(outBytes, 0, outBytes.Length);
                }
            }
        }


        /*
        /// <summary>
        /// This is the main entry point for your service's partition replica. 
        /// RunAsync executes when the primary replica for this partition has write status.
        /// </summary>
        /// <param name="cancelServicePartitionReplica">Canceled when Service Fabric terminates this partition's replica.</param>
        protected override async Task RunAsync(CancellationToken cancelServicePartitionReplica)
        {
            // TODO: Replace the following sample code with your own logic.

            // Gets (or creates) a replicated dictionary called "myDictionary" in this partition.
            var voteDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            // This partition's replica continues processing until the replica is terminated.
            while (!cancelServicePartitionReplica.IsCancellationRequested)
            {

                // Create a transaction to perform operations on data within this partition's replica.
                using (var tx = this.StateManager.CreateTransaction())
                {

                    // Try to read a value from the dictionary whose key is "Counter-1".
                    var result = await voteDictionary.TryGetValueAsync(tx, "Counter-1");

                    // Log whether the value existed or not.
                    ServiceEventSource.Current.ServiceMessage(this, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    // If the "Counter-1" key doesn't exist, set its value to 0
                    // else add 1 to its current value.
                    await voteDictionary.AddOrUpdateAsync(tx, "Counter-1", 0, (k, v) => ++v);

                    // Committing the transaction serializes the changes and writes them to this partition's secondary replicas.
                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is sent to this partition's secondary replicas.
                    await tx.CommitAsync();
                }

                // Pause for 1 second before continue processing.
                await Task.Delay(TimeSpan.FromSeconds(1), cancelServicePartitionReplica);
            }
        }*/
    }
}
