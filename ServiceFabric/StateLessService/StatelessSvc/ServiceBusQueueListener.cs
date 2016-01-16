using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;

namespace StatelessSvc
{
    public class ServiceBusQueueListener : ICommunicationListener
    {
        private StatelessServiceInitializationParameters m_Args;
        private StatelessService m_Service;

        public ServiceBusQueueListener(StatelessService service, StatelessServiceInitializationParameters args)
        {
            this.m_Service = service;
            this.m_Args = args;
        }

        public void Abort()
        {
            ServiceEventSource.Current.ServiceMessage(m_Service, "ServiceBusQueueListener.Abort invoked ");
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(m_Service, "ServiceBusQueueListener.CloseAsync invoked ");
            return Task.Run(() =>{ });
        }

 
        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(m_Service, "ServiceBusQueueListener.OpenAsync invoked ");

            return Task<string>.Run(() => { return "samplequeue"; });
        }
    }
}
