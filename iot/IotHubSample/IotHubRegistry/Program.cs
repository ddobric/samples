using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHubSample
{
    class Program
    {
        static RegistryManager m_RegistryManager;

        static string m_RegistryConnStr;

        static void Main(string[] args)
        {
            m_RegistryConnStr = ConfigurationManager.AppSettings["IotHub.RegistryConnStr"];
            m_RegistryManager = RegistryManager.CreateFromConnectionString(m_RegistryConnStr);
           // AddDeviceAsync().Wait();
            EnlistDevices().Wait();

            Console.ReadLine();
        }

        private async static Task AddDeviceAsync()
        {
            string deviceId = "D001";
            Device device;
            try
            {
                device = await m_RegistryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await m_RegistryManager.GetDeviceAsync(deviceId);
            }

            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }

        private async static Task EnlistDevices()
        {

            var devices = await m_RegistryManager.GetDevicesAsync(int.MaxValue);
            foreach (var device in devices)
            {
                Console.WriteLine("------------------------------");
                Console.WriteLine("GenId:{0}, Id:{1}\r\nKey:{2}\r\nCommands:{3}, ConnState:{4}\r\nLast Activity Time:{5}\r\nStatus:{6}, Reason{7}, StatusUpdate:{8}",
                     device.GenerationId,
                    device.Id,
                    device.Authentication.SymmetricKey.PrimaryKey,
                    device.CloudToDeviceMessageCount,
                    device.ConnectionState,

                    device.LastActivityTime,
                    device.Status,
                    device.StatusReason,
                    device.StatusUpdatedTime);
            }


        }
    }
}
