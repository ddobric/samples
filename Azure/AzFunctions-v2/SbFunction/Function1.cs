using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace SbFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([ServiceBusTrigger("myqueue", Connection = "Endpoint=sb://students.servicebus.windows.net/;SharedAccessKeyName=bastafunctions;SharedAccessKey=W1HMXje0prjK/S/8HznJobObho8TjDHgPSW3Gxb9gvo=")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
