//#define LOCAL_EXEC
namespace FunctionRecognizer
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using LearningFoundation;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using AnomDetect.KMeans.FunctionRecognition;
    using Newtonsoft.Json;
    using LearningFoundation.Helpers;

    class Program
    {
        static int counter;

        static void Main(string[] args)
        {
            // The Edge runtime gives us the connection string we need -- it is injected as an environment variable
            string connectionString = Environment.GetEnvironmentVariable("EdgeHubConnectionString");

            // Cert verification is not yet fully functional when using Windows OS for the container
            bool bypassCertVerification = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!bypassCertVerification) InstallCert();
            Init(connectionString, bypassCertVerification).Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Add certificate in local cert store for use by client for secure connection to IoT Edge runtime
        /// </summary>
        static void InstallCert()
        {
            string certPath = Environment.GetEnvironmentVariable("EdgeModuleCACertificateFile");
            if (string.IsNullOrWhiteSpace(certPath))
            {
                // We cannot proceed further without a proper cert file
                Console.WriteLine($"Missing path to certificate collection file: {certPath}");
                throw new InvalidOperationException("Missing path to certificate file.");
            }
            else if (!File.Exists(certPath))
            {
                // We cannot proceed further without a proper cert file
                Console.WriteLine($"Missing path to certificate collection file: {certPath}");
                throw new InvalidOperationException("Missing certificate file.");
            }
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(certPath)));
            Console.WriteLine("Added Cert: " + certPath);
            store.Close();
        }


        /// <summary>
        /// Initializes the DeviceClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init(string connectionString, bool bypassCertVerification = false)
        {
            Console.WriteLine("Connection String {0}", connectionString);
#if !LOCAL_EXEC
            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            // During dev you might want to bypass the cert verification. It is highly recommended to verify certs systematically in production
            if (bypassCertVerification)
            {
                mqttSetting.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }
            ITransportSettings[] settings = { mqttSetting };

            Console.WriteLine("Before openasync.");
            // Open a connection to the Edge runtime
            DeviceClient ioTHubModuleClient = DeviceClient.CreateFromConnectionString(connectionString, settings);
            await ioTHubModuleClient.OpenAsync();

            Console.WriteLine("IoT Hub module client initialized.");

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("inputAnomalyDetector", pipeMessage, ioTHubModuleClient);

            Console.WriteLine("Handler initialized.");
#else
            var funcData = FunctionGenerator.CreateFunction(500, 2, 2 * Math.PI / 100);

            // Simulates message receiving.
            while (true)
            {             
                var similarFunc = FunctionGenerator.CreateSimilarFromReferenceFunc(funcData.ToArray(), 15);

                var msgData = JsonConvert.SerializeObject(similarFunc);

                Message msg = new Message(UTF8Encoding.UTF8.GetBytes(msgData));

                var res = await pipeMessage(msg, null);

                Console.WriteLine($"Generated function: {msgData.Length}. Data sent to local IotHub. Noice:{10}");

                await Task.Delay(1000);
            }
            //await Task.Run(() => { });
#endif

        }


        /// <summary>
        /// Validate anomalies in the function data.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isAnomaly"></param>
        /// <returns></returns>
        private static double[][] processMessage(Message message, out bool isAnomaly)
        {
            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);
            double[][] funcData = formatData(JsonConvert.DeserializeObject<double[][]>(messageString));

            var api = LearningApi.Load("sinusmodel");
            var res = api.Algorithm.Predict(funcData, null) as KMeansFuncionRecognitionResult;
            if (res.Result == false)
                isAnomaly = true;
            else
                isAnomaly = false;

            return funcData;
        }


        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        static async Task<MessageResponse> pipeMessage(Message message, object userContext)
        {
            int counterValue = Interlocked.Increment(ref counter);

            var deviceClient = userContext as DeviceClient;
            if (deviceClient == null)
            {
                // throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            Console.WriteLine($"Received message: {counterValue}");

            bool isAnomaly;

            Message newMsg = new Message();
            foreach (var prop in message.Properties)
            {
                newMsg.Properties.Add(prop.Key, prop.Value);
            }

            var funcData = processMessage(message, out isAnomaly);
            if (isAnomaly)
            {
                if (deviceClient != null)
                    await deviceClient.SendEventAsync("anomalyOutput", newMsg);

                Console.WriteLine("*** Anomaly detected!!!");
            }
            else
            {
                if (deviceClient != null)
                    await deviceClient.SendEventAsync("output", newMsg);

                Console.WriteLine("Function has no anomalies.");
            }

            return MessageResponse.Completed;
        }

        private static double[][] formatData(double[][] similarFuncData)
        {
            double[][] data = new double[similarFuncData[0].Length][];
            for (int i = 0; i < similarFuncData[0].Length; i++)
            {
                data[i] = new double[similarFuncData.Length];
                for (int j = 0; j < similarFuncData.Length; j++)
                {
                    data[i][j] = similarFuncData[j][i];
                }
            }

            return data;
        }
    }
}
