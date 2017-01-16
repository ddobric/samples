using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Diagnostics;
using System.Fabric;
using System.Fabric.Description;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace VotingService
{
    public sealed class HttpCommunicationListener : ICommunicationListener
    {
        private readonly String m_PublishedUri;
        private readonly HttpListener m_HttpListener = new HttpListener();
        private readonly Func<HttpListenerContext, CancellationToken, Task> m_ProcessRequestFunc;
        private readonly CancellationTokenSource m_processRequestsCancellation = new CancellationTokenSource();

        public HttpCommunicationListener(StatefulServiceContext args, string endpointName,
            Func<HttpListenerContext, CancellationToken, Task> processRequestFunc)
        {
            m_ProcessRequestFunc = processRequestFunc;
            // Partition replica's URL is the node's IP, desired port, PartitionId, ReplicaId, Guid
            EndpointResourceDescription internalEndpoint = args.CodePackageActivationContext.GetEndpoint(endpointName);
            var uriPrefix = $"{internalEndpoint.Protocol}://+:{internalEndpoint.Port}/"
               + $"{args.PartitionId}/{args.ReplicaId}"
               + $"-{Guid.NewGuid()}/";   // Uniqueness
            m_HttpListener.Prefixes.Add(uriPrefix);
            m_PublishedUri = uriPrefix.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

          Debug.WriteLine(m_PublishedUri);
        }

        public void Abort()
        {
            m_processRequestsCancellation.Cancel(); m_HttpListener.Abort();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            m_processRequestsCancellation.Cancel();
            m_HttpListener.Close(); return Task.FromResult(true);
        }
        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            m_HttpListener.Start();
            var noWarning = ProcessRequestsAsync(m_processRequestsCancellation.Token);
            return Task.FromResult(m_PublishedUri);
        }
        private async Task ProcessRequestsAsync(CancellationToken processRequests)
        {
            while (!processRequests.IsCancellationRequested)
            {
                HttpListenerContext request = await m_HttpListener.GetContextAsync();

                // The ContinueWith forces rethrowing the exception if the task fails.
                var noWarning = m_ProcessRequestFunc(request, m_processRequestsCancellation.Token)
                    .ContinueWith(async t => await t /* Rethrow unhandled exception */, TaskContinuationOptions.OnlyOnFaulted);
            }
        }
    }
}
