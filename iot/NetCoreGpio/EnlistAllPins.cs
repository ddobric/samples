using System;
using System.Collections.Generic;
using System.Text;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace NetCoreGpio
{
    /// <summary>
    /// Enumerates all GPIO ports to find out the right mapping.
    /// </summary>
    internal class EnlistAllPins
    {
        static void Go(string[] args)
        {
            Console.WriteLine("Hello from .NET Core PI!");

            int[] a = new int[12];

            // Get a reference to the pin you need to use.
            // All 3 methods below are exactly equivalent
            //var blinkingPin = Pi.Gpio[17];
            //blinkingPin = Pi.Gpio.Pin00;
            GpioPin[] pins = new GpioPin[30];

            for (int i = 0; i < 30; i++)
            {
                try
                {
                    pins[i] = Pi.Gpio[i];
                    pins[i].PinMode = GpioPinDriveMode.Output;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


            //var blinkingPin = Pi.Gpio[WiringPiPin.Pin17];            

            //// Configure the pin as an output
            //blinkingPin.PinMode = GpioPinDriveMode.Output;

            // perform writes to the pin by toggling the isOn variable
            var isOn = false;
            for (var i = 0; i < 200; i++)
            {

                isOn = !isOn;
                for (int j = 0; j < pins.Length; j++)
                {
                    try
                    {
                        Console.WriteLine($"pin: {j} - {isOn}");
                        pins[j].Write(isOn);
                        Console.ReadKey();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"err: {j}");
                    }
                }
                //blinkingPin.Write(isOn);
                System.Threading.Thread.Sleep(1500);
            }
        }

    }
}

