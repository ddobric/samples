using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using ServiceBus.OpenSdk;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pi2Led
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GpioController m_Gpio;
        private bool m_Voltage = false;
        private GpioPin m_Pin27;
        private GpioPin m_Pin22;


        public MainPage()
        {
            this.InitializeComponent();

            m_GpioStatus.Text = "Connecting to GPIO.";

            try
            {
                // This should return default GPIP controller.
                m_Gpio = GpioController.GetDefault();

                //
                // If the GPIO controller is not present NULL is returned.
                // This might be the case, when app is running on phone or XBOX-One
                if (m_Gpio != null)
                {
                    initializeGpioPins();
                }
                else
                    m_GpioStatus.Text = "No GPIO controller detected.";

                startSB();
                //startIoTHub();
                //startLedSwitcher();

                ////
                //// Here we change LED status directly.
                //Task.Run(() =>
                //{
                //    changeLedStatus(null, null);
                //});
            }
            catch (Exception)
            {
                startSB();
                //startIoTHub();
            }
        }

        private void initializeGpioPins()
        {
            //
            // Here we open GPIO 22 and 27
            //
            m_Pin27 = m_Gpio.OpenPin(27);

            m_Pin22 = m_Gpio.OpenPin(22);

            //
            // The we set both port to output mode.
            //

            m_Pin27.SetDriveMode(GpioPinDriveMode.Output);

            m_Pin22.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void startLedSwitcher()
        {
            //
            // This timer is changing LED status.
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(2);
            t.Tick += onTimer;
            t.Start();
        }

    
        private void onTimer(object sender, object e)
        {
            changeLedStatus(null, null);
        }

        private async void changeLedStatus(object sender, RoutedEventArgs e)
        {
            m_Voltage = !m_Voltage;

            if (m_Voltage)
            {
                if (m_Gpio != null)
                {
                    m_Pin27.Write(GpioPinValue.High);
                    m_Pin22.Write(GpioPinValue.High);
                }
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    m_GpioStatus.Text = "ON";
                });

                await sendTelemetryData();
            }
            else
            {
                if (m_Gpio != null)
                {
                    m_Pin27.Write(GpioPinValue.Low);
                    m_Pin22.Write(GpioPinValue.Low);
                }

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    m_GpioStatus.Text = "OFF";
                });
            }
        }

        #region Service Bus Messaging

        private QueueClient m_QueueClient;

        private DispatcherTimer m_CmdReceiverTimer;

        /// <summary>
        /// Start loop for receiving commands from Service Bus queue.
        /// </summary>
        private void startSB()
        {
            initSbClient();

            m_CmdReceiverTimer = new DispatcherTimer();
            m_CmdReceiverTimer.Interval = TimeSpan.FromSeconds(5);
            m_CmdReceiverTimer.Tick += onTelemetryTimer;
            m_CmdReceiverTimer.Start();
        }



        private void onTelemetryTimer(object sender, object e)
        {
            // Decide here what you want to do.
            receiveCommandFromSb();
            sendTelemetryDataToSb();
        }

        private async void receiveCommandFromSb()
        {
           // m_CmdReceiverTimer.Stop();
            await readCommand();
            //m_CmdReceiverTimer.Start();
        }

        private async void sendTelemetryDataToSb()
        {
            await sendTelemetryData();
        }


        /// <summary>
        /// Reads the command from Service Bus queue and executes it.
        /// If 'on' then switch ON the LED. Otherwise swithc OFF the LED.
        /// </summary>
        /// <returns></returns>
        private async Task readCommand()
        {
            try
            {
                var cmdMsg = await m_QueueClient.Receive(ReceiveMode.ReceiveAndDelete);
                if (cmdMsg == null) return;

                if (cmdMsg.Properties.ContainsKey("Command"))
                {
                    string cmd = cmdMsg.Properties["Command"] as string;

                    m_GpioStatus.Text = String.Format("Received command '{0}'", cmd);

                    if (m_Pin27 != null && m_Pin22 != null)
                    {
                        if (cmd.ToLower() == "on")
                        {
                            m_Pin27.Write(GpioPinValue.High);
                            m_Pin22.Write(GpioPinValue.High);
                        }
                        else
                        {
                            m_Pin27.Write(GpioPinValue.Low);
                            m_Pin22.Write(GpioPinValue.Low);
                        }
                    }
                }
                else
                    m_GpioStatus.Text = "Unknown command received!";
            }
            catch (Exception ex)
            {

            }
        }

        EventHubClient m_EventHubClient;

        /// <summary>
        /// Initializes Serbice Bus communication
        /// </summary>
        private void initSbClient()
        {
            var tokenHub = new SASTokenProvider("devicesend", "CicouVjHjl7g5S/y/ViMRSjGU29eCyNaKBAklfT+QvU=");
            var tokenQueue = new SASTokenProvider("device", "pvzRq8Hed1xfAhx2OZABppG8e5dnDu0xpWvzZypBCiw=");
            
            m_QueueClient = new QueueClient("studentprojects", "commandqueue", tokenQueue, "http");
            m_EventHubClient = new EventHubClient("studentprojects", "msbandaccelerator", tokenHub, "http");
        }


        /// <summary>
        /// Sends telemetry data to SB. We simulate here temperature sensor.
        /// </summary>
        /// <returns></returns>
        private async Task sendTelemetryData()
        {
            //initSbClient();

            try
            {
                var tmp = getTemperature();

                ServiceBus.OpenSdk.Message msg = new ServiceBus.OpenSdk.Message(
                    new Command
                    {
                        DeviceName = "PI2-DAENET-GmbH",
                        Temperature = tmp,
                    });                

                await m_EventHubClient.Send(msg);

                Debug.WriteLine(tmp);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private Random m_Rnd = new Random();


        private double getTemperature()
        {
            double[] t = new double[] { 21.2, 20.9, 20.8, 21.0, 21.1, 29.2, 31.4 };
           // double[] t = new double[] { 26.2, 29.9, 31.8, 31.0, 30.1, 29.2, 31.4 };

            return t[m_Rnd.Next(6)];
        }
        #endregion

        #region IOT HUB Mesaging

        static string m_ConnStr = "HostName=daenethub.azure-devices.net;SharedAccessKeyName=ddc;SharedAccessKey=NuxNFBI4pmral+MhYUxgvckHJ2U2DMRxcJovCKPkhfI=";
        static DeviceClient m_DeviceClient = DeviceClient.CreateFromConnectionString(m_ConnStr, "D001", TransportType.Http1);


        /// <summary>
        /// Start sending of messages to IoTHub and loop for receiving of commands from hub.
        /// </summary>
        private void startIoTHub()
        {
            m_GpioStatus.Text = "There is no GPIO controller on this device.";

            m_CmdReceiverTimer = new DispatcherTimer();
            m_CmdReceiverTimer.Interval = TimeSpan.FromSeconds(5);
            m_CmdReceiverTimer.Tick += onIoTHubTimer;
            m_CmdReceiverTimer.Start();
        }

        private void onIoTHubTimer(object sender, object e)
        {
            //Change here what you want to do.
            sendMessageToIoTHub();
            //receiveCommandsLoop();
        }

        private static async void sendMessageToIoTHub()
        {
            try {
                double avgWindSpeed = 10; // m/s
                Random rand = new Random();

                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = "D001",
                    Temperature = currentWindSpeed,
                    Current = currentWindSpeed
                };

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);

                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));

                await m_DeviceClient.SendEventAsync(message);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        static async Task receiveIotHubCommandsLoop()
        {
            //Console.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Microsoft.Azure.Devices.Client.Message receivedMessage;
            string messageData;
            while (true)
            {
                try
                {
                    receivedMessage = await m_DeviceClient.ReceiveAsync(TimeSpan.FromSeconds(5));

                    if (receivedMessage != null)
                    {
                        messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                        Debug.WriteLine(receivedMessage);
                        //    Console.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);
                        await m_DeviceClient.CompleteAsync(receivedMessage);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        #endregion
    }
}
