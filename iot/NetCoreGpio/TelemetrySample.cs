using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace NetCoreGpio
{
    internal class TelemetrySample
    {
        public static async Task RunAync(string[] args)
        {
            DeviceClient client = DeviceClient.CreateFromConnectionString(Program.ConnStr);

            Console.WriteLine("Hello from .NET Core PI telemetry sender!");

            int gpioPort = 24;

            if (args != null && args.Length == 1)
                int.TryParse(args[0], out gpioPort);

            Console.WriteLine($"Taking control over pin {gpioPort}");

            GpioPin pin = Pi.Gpio[gpioPort];

            pin.PinMode = GpioPinDriveMode.Input;

            while (true)
            {
                var pinState = pin.Read();

                Console.Write($"{pinState}");

                var msg = new { GpioState = pinState, GpioPort = gpioPort };

                await client.SendEventAsync(new Message(UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg))));

                Console.WriteLine("Event sent to IotHub."); 

                Thread.Sleep(1500);
            }
        }
    }
}
