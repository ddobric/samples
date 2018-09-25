using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace NetCoreGpio
{
    internal class GpioSample
    {
        public static Task RunAync(string[] args)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Hello from .NET Core PI!");

                int gpioPort = 24;

                if (args != null && args.Length == 1)
                    int.TryParse(args[0], out gpioPort);

                Console.WriteLine($"Taking control over pin {gpioPort}");

                GpioPin pin = Pi.Gpio[gpioPort];

                pin.PinMode = GpioPinDriveMode.Output;

                var pinState = false;

                for (var i = 0; i < 200; i++)
                {
                    pinState = !pinState;

                    Console.WriteLine($"pin: {gpioPort} - {pinState}");

                    System.Threading.Thread.Sleep(1000);

                    pin.Write(pinState);
                }
            });
        }
    }
}
