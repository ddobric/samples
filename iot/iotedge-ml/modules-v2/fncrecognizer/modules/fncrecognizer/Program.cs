//#define LOCAL_EXEC
namespace fncrecognizer
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AnomDetect.KMeans.FunctionRecognition;
    using LearningFoundation;
    using LearningFoundation.Helpers;
    using Microsoft.Azure.Devices.Client;
    using Newtonsoft.Json;

    class Program
    {
        static int counter;

        static void Main(string[] args)
        {
            Init().Wait();

            Console.WriteLine("Waiting on messages...");

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();

            Console.WriteLine("Exit.");
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

        private static ModuleClient ioTHubModuleClient;

        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init()
        {
            AmqpTransportSettings amqpSetting = new AmqpTransportSettings(TransportType.Amqp_Tcp_Only);
            ITransportSettings[] settings = { amqpSetting };

            // Open a connection to the Edge runtime
            ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync();

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("input1", PipeMessage, ioTHubModuleClient);


#if !LOCAL_EXEC

            Console.WriteLine("IoT Hub module client initialized.");
#else
            var funcData = FunctionGenerator.CreateFunction(500, 2, 2 * Math.PI / 100);

            // Simulates message receiving.
            while (true)
            {
                var similarFunc = FunctionGenerator.CreateSimilarFromReferenceFunc(funcData.ToArray(), 5);

                var msgData = JsonConvert.SerializeObject(similarFunc);

                Message msg = new Message(UTF8Encoding.UTF8.GetBytes(msgData));

                var res = await PipeMessage(msg, null);

                Console.WriteLine($"Generated function: {msgData.Length}. Data sent to local IotHub. Noice:{10}");

                await Task.Delay(1000);
            }
            //await Task.Run(() => { });
#endif
        }

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        static async Task<MessageResponse> PipeMessage(Message message, object userContext)
        {
            int counterValue = Interlocked.Increment(ref counter);

            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
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

                await moduleClient.SendEventAsync("anomalyoutput", newMsg);

                Console.WriteLine($"{DateTime.Now} *** Anomaly detected!!!");
            }
            else
            {

                await moduleClient.SendEventAsync("telemetryoutput", newMsg);

                Console.WriteLine($"{DateTime.Now} - Function has no anomalies.");
            }

            return MessageResponse.Completed;
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

            writeResult(res);

            if (res.Result == false)
                isAnomaly = true;
            else
                isAnomaly = false;

            return funcData;
        }

        private static void writeResult(KMeansFuncionRecognitionResult res)
        {
            foreach (var item in res.ResultsPerCluster)
            {
                Console.Write($"{item} - ");
            }

            Console.WriteLine();
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
