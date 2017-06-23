using Iot;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Iot.PhilipsHueConnector.Entities;

namespace PhilipsHue.DesktopGateway
{
    class Program
    {
        /// <summary>
        /// URL of the Hue gateway.
        /// </summary>
        private static string m_GtwUri = "http://192.168.100.100/";//"http://192.168.178.40"

        /// <summary>
        /// To set username, you first have to run test GenerateUserTest().
        /// This method will connect to Hue Gateway. BEfore you run it, click 
        /// the link button on the gatewey. Method GenerateUserName will return
        /// username, which you should set as value of this member variable.
        /// </summary>
        private static string m_UsrName = "IAFHa0gHSQwO4vHcKLi45U4GIOT8Ldky0nLdN9bC";//"LjRTgTUSLZmzodtrmKK1TxTlN8vZdaQ-fFMOllxt"

        private static string m_ConnStr = "HostName=daenethub.azure-devices.net;DeviceId=D001;SharedAccessKey=uck7p18XBxCSQBxe0sNeoAyruqSy9HpDTIA2+ba1jy8=";

        private static IotApi m_Api;

        private static DeviceClient m_DeviceClient;

        static void Main(string[] args)
        {
            m_DeviceClient = DeviceClient.CreateFromConnectionString(m_ConnStr, TransportType.Mqtt);

            runHueGateway();

            runIotHubMethodListener();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("daenet PhilipsHue Gateway router is connected to IoTHub.");
            
            Console.WriteLine("IotApi initialized.");

            Console.ReadLine();
        }

        private static void runHueGateway()
        {
            m_Api = new IotApi();
            m_Api.UsePhilpsQueueRest(m_GtwUri, m_UsrName);
            m_Api.Open();
        }

        private static async Task<MethodResponse> routeRequest(MethodRequest request, object args)
        {
            MethodResponse resp = null;
          
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Received message: ");
                Console.Write(request.DataAsJson);

                JObject obj = JsonConvert.DeserializeObject(request.DataAsJson) as JObject;
                if (obj != null)
                {
                    JToken jMethod = lookupValue(obj, "method");
                    string method = jMethod.Value<string>();

                    JToken jUri = lookupValue(obj, "uri");
                    string uri = jUri.Value<string>();

                    JToken jBody = lookupValue(obj, "body");
                 
                    HueCommand command = new HueCommand();
                    command.Path = uri;
                    command.Method = method;
                    command.Body = jBody.ToString();

                    Console.WriteLine($"Invoking operation: {command.Method} - {command.Path}");

                    var hueResult = await m_Api.SendAsync(command);

                    Console.WriteLine(hueResult);

                    resp = getResponse(hueResult.ToString(), 200);
                }
                else
                {
                    Console.WriteLine($":( Unknown request. {request.DataAsJson}");
                    resp = getResponse(":( Unknown request", 201);
                }             
            }
            catch (IotApiException iotEx)
            {
                Exception ex = iotEx;

                while (ex.InnerException != null)
                {
                    if (ex.InnerException as IotApiException != null && ((IotApiException)ex).ReceivedMessages != null)
                    {
                        ex = ex.InnerException;
                        break;
                    }
                        
                    else
                        ex = ex.InnerException;                    
                }

                if (ex is IotApiException)
                {
                    Console.WriteLine(((IotApiException)ex).ReceivedMessages[0].ToString());
                    resp = getResponse($"{ex.Message} - {((IotApiException)ex).ReceivedMessages[0].ToString()}", 202);
                }
                else if (ex != null)
                {
                    Console.WriteLine(iotEx.ReceivedMessages[0].ToString());
                    resp = getResponse($"{ex.Message}", 202);
                }
                else
                {
                    Console.WriteLine(((IotApiException)ex).ReceivedMessages[0].ToString());
                    resp = getResponse($"{iotEx.Message} - {iotEx.ReceivedMessages[0].ToString()}", 202);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                resp = getResponse(ex.ToString(), 203);
            }

            return resp;
        }


        private static MethodResponse getResponse(string msg, int code)
        {
            return new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg)), code);
        }

        private async static void runIotHubMethodListener()
        {
            MethodCallback methodCallback = new MethodCallback(routeRequest);
            
            await m_DeviceClient.OpenAsync();

            m_DeviceClient.SetMethodHandler("RouteRequest", methodCallback, "context");
        }

        private static JToken lookupValue(JObject item, string propName)
        {
            foreach (JProperty prop in item.Properties())
            {
                if (prop.Name.ToLower() == propName)
                {
                    return prop.Value;
                }
            }


            return null;
        }
    }
}
