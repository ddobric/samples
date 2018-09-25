#define WINDOWS2
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if !WINDOWS
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
#endif

namespace NetCoreGpio
{
    internal class WebMethods
    {
#if !WINDOWS
        private static GpioPin pin;
#endif
        public static Task RunAync(string[] args)
        {
            return Task.Run(async()=>{
                DeviceClient client = DeviceClient.CreateFromConnectionString(Program.ConnStr, TransportType.Mqtt);

                Console.WriteLine("Hello from .NET Core PI!");

#if !WINDOWS
                int gpioPort = 24;

                if (args != null && args.Length == 1)
                    int.TryParse(args[0], out gpioPort);

                pin = Pi.Gpio[gpioPort];

                pin.PinMode = GpioPinDriveMode.Output;
#endif
                MethodCallback methodCallback = new MethodCallback(handlerFnc);

                await client.OpenAsync();

                await client.SetMethodHandlerAsync("RouteRequest", methodCallback, "context");

                Console.WriteLine("Web method handler initialized.");

                Console.ReadKey();
            });
        }

        private static Task<MethodResponse> handlerFnc(MethodRequest request, object args)
        {
            try
            {
                Console.WriteLine($"New request received. {request.DataAsJson}");

                var dynReq = JsonConvert.DeserializeObject<dynamic>(request.DataAsJson);

                Console.WriteLine($"Received payload. {dynReq}");

                string response = JsonConvert.SerializeObject(new
                {
                    Time = DateTime.Now,
                    PinState = dynReq.state
                });

#if !WINDOWS
                pin.Write(dynReq.state.Value);
#endif
                Console.WriteLine($"Status set to {dynReq.state.Value}");

                MethodResponse resp = new MethodResponse(Encoding.UTF8.GetBytes(response), 200);

                return Task.FromResult<MethodResponse>(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                throw;
            }
        }

    }
}
