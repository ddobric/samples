using Sample01;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TemperatureReader.Logic.Devices;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
//http://dotnetbyexample.blogspot.de/2015/09/reading-temperatures-controlling-fan.html
//http://raspberrypi.stackexchange.com/questions/33010/how-to-read-analog-5v-sensor-ouput-with-digital-3-3v-gpio


namespace PiTemperature
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private EventHubWriter m_Writer;

        private GpioPin m_Pin1;
        private GpioPin m_Pin2;

        public MainPage()
        {
            this.InitializeComponent();

            runAsync();
        }

        private async void runAsync()
        {
            var gpioCtrl = GpioController.GetDefault();
            if (gpioCtrl != null)
            {
                m_Writer = new EventHubWriter();

                await m_Writer.Open(new Dictionary<string, object>() {
                { "Namespace", "studentprojects" },
                { "DeviceId", "dev01" },
                { "Key", "CicouVjHjl7g5S/y/ViMRSjGU29eCyNaKBAklfT+QvU=" },
                { "KeyName", "devicesend" },
                { "Hub", "msbandaccelerator" },
            });

                await initLed(gpioCtrl);

                AnalogDigitalController m_AdcController = new AnalogDigitalController((val) =>
                {
                    try
                    {
                        var t = Math.Round(((255 - val) - 121) * 0.21875, 1) + 21.8;
                        Debug.WriteLine(t);

                        m_Pin2.Write(GpioPinValue.High);
                        Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
                        m_Pin2.Write(GpioPinValue.Low);

                        var sensorEvent = new
                        {
                            sensor = "PiTemperature",
                            deviceId = "Pi2",
                            Temperature = t,
                            ts = DateTime.Now,
                        };

                        writeToCloudWithRetry(sensorEvent).Wait();

                        m_Pin2.Write(GpioPinValue.High);
                        Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
                        m_Pin2.Write(GpioPinValue.Low);
                    }
                    catch (Exception ex)
                    {
                        m_Pin2.Write(GpioPinValue.Low);
                        m_Pin1.Write(GpioPinValue.High);
                        Task.Delay(TimeSpan.FromSeconds(3)).Wait();
                        m_Pin1.Write(GpioPinValue.Low);
                    }
                });


                m_AdcController.Start();
            }
        }

        private Task writeToCloudWithRetry(dynamic sensorEvent)
        {
            int retries = 10;

            while (true)
            {
                try
                {
                    var t = m_Writer.WriteToStream(sensorEvent);

                    t.Wait();

                    return t;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException?.Message == "Unauthorized")
                    {
                        if (retries < 0)
                            throw ex;

                        retries--;

                        for (int i = 0; i < 10; i++)
                        {
                            m_Pin1.Write(GpioPinValue.High);
                            Task.Delay(500).Wait();
                            m_Pin1.Write(GpioPinValue.Low);
                            Task.Delay(500).Wait();
                        }                       
                    }

                    else
                        throw ex;
                }
            }
        }

        private async Task initLed(GpioController gpioCtrl)
        {
            m_Pin1 = gpioCtrl.OpenPin(22);
            m_Pin2 = gpioCtrl.OpenPin(27);
            m_Pin1.SetDriveMode(GpioPinDriveMode.Output);
            m_Pin2.SetDriveMode(GpioPinDriveMode.Output);

            m_Pin1.Write(GpioPinValue.Low);
            m_Pin2.Write(GpioPinValue.Low);

            for (int i = 0; i < 3; i++)
            {
                m_Pin1.Write(GpioPinValue.High);
                await Task.Delay(TimeSpan.FromMilliseconds(250));
                m_Pin1.Write(GpioPinValue.Low);
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }

            for (int i = 0; i < 3; i++)
            {
                m_Pin2.Write(GpioPinValue.High);
                await Task.Delay(TimeSpan.FromMilliseconds(250));
                m_Pin2.Write(GpioPinValue.Low);
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }
        }
    }
}
