using Iot;
using Iot.PhilipsHueConnector;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilipsHue.NetCoreGateway
{
    public class Program
    {
        /// <summary>
        /// URL of the Hue gateway.
        /// </summary>
        private static string m_GtwUri = "http://192.168.?.?/";

        /// <summary>
        /// To set username, you first have to run test GenerateUserTest().
        /// This method will connect to Hue Gateway. BEfore you run it, click 
        /// the link button on the gatewey. Method GenerateUserName will return
        /// username, which you should set as value of this member variable.
        /// </summary>
        private static string m_UsrName = "";

        private static DeviceClient m_DeviceClient;

        static void Main(string[] args)
        {
            var connStr = "TODO Connection string for IoTHub";

            m_DeviceClient = DeviceClient.CreateFromConnectionString(connStr, TransportType.Mqtt);

            runMethodListener();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("daenet PhilipsHue Gateway router is connected to IoTHub.");

            IotApi api = new IotApi();
            api.UsePhilpsQueueRest(m_GtwUri, m_UsrName);
            api.Open();

            Console.WriteLine("IotApi initialized.");

            Console.ReadLine();
        }

        private static Task<MethodResponse> routeRequest(MethodRequest request, object args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Received message: ");
            Console.Write(request.DataAsJson);

            // api.SendAsync()
            string jsonResp = JsonConvert.SerializeObject(new
            {
                DeviceStatus = "on",
            });

            MethodResponse resp = new MethodResponse(Encoding.UTF8.GetBytes(jsonResp), 200);

            return Task.FromResult<MethodResponse>(resp);
        }

        private async static void runMethodListener()
        {
            MethodCallback methodCallback = new MethodCallback(routeRequest);

            await m_DeviceClient.OpenAsync();

            m_DeviceClient.SetMethodHandler("RouteRequest", methodCallback, "context");
        }

        private static JToken lookupValue(JArray arr, string propName)
        {
            foreach (var item in arr.Children<JObject>())
            {
                foreach (JProperty prop in item.Properties())
                {
                    if (prop.Name.ToLower() == propName)
                    {
                        return prop.Value;
                    }
                }
            }

            return null;
        }
    }
}
