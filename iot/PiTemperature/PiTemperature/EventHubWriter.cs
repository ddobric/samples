
using Newtonsoft.Json;
using ServiceBus.OpenSdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;

namespace Sample01
{
    public class EventHubWriter
    {
        private EventHubClient m_EventHubClient;

        private string m_DeviceId;


        public string Name
        {
            get
            {
                return "EventtHubWriter";
            }
        }

        public async Task WriteToStream(dynamic sensorEvent)
        {
            var stringified = JsonConvert.SerializeObject(sensorEvent);

            var bytes = Encoding.UTF8.GetBytes(stringified);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                stream.Flush();
                var message = new ServiceBus.OpenSdk.Message(stream);
               
                await m_EventHubClient.Send(message);
            }
        }

        public Task Open(Dictionary<string, object> args)
        {
            string ns = null;
            string hub = null;
            string keyname = null;
            string key = null;
            string deviceId = null;

            if (args != null)
            {
                if (args.ContainsKey("Namespace"))
                    ns = (string)args["Namespace"];

                if (args.ContainsKey("Hub"))
                    hub = (string)args["Hub"];

                if (args.ContainsKey("KeyName"))
                    keyname = (string)args["KeyName"];

                if (args.ContainsKey("Key"))
                    key = (string)args["Key"];

                if (args.ContainsKey("DeviceId"))
                    deviceId = (string)args["DeviceId"];
            }

            if (hub == null)
                throw new Exception("EventHub must be provided.");

            if (key == null)
                throw new Exception("Key must be provided.");

            if (keyname == null)
                throw new Exception("KeyName must be provided.");

            if (ns == null)
                throw new Exception("Namespace must be provided.");

            if (deviceId == null)
                throw new Exception("DeviceId must be provided.");

            var tokenProv = new SASTokenProvider(keyname, key);
            m_EventHubClient = new EventHubClient(ns, hub, tokenProv, "http");
            m_DeviceId = deviceId;

            return Task.FromResult<bool>(false);
        }

        public async Task Close()
        {
            throw new NotSupportedException();
        }
    }

}

