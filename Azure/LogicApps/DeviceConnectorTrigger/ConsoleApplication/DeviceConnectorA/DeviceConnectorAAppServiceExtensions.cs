using System;
using System.Net.Http;
using Microsoft.Azure.AppService;

namespace ConsoleApplication
{
    public static class DeviceConnectorAAppServiceExtensions
    {
        public static DeviceConnectorA CreateDeviceConnectorA(this IAppServiceClient client)
        {
            return new DeviceConnectorA(client.CreateHandler());
        }

        public static DeviceConnectorA CreateDeviceConnectorA(this IAppServiceClient client, params DelegatingHandler[] handlers)
        {
            return new DeviceConnectorA(client.CreateHandler(handlers));
        }

        public static DeviceConnectorA CreateDeviceConnectorA(this IAppServiceClient client, Uri uri, params DelegatingHandler[] handlers)
        {
            return new DeviceConnectorA(uri, client.CreateHandler(handlers));
        }

        public static DeviceConnectorA CreateDeviceConnectorA(this IAppServiceClient client, HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
        {
            return new DeviceConnectorA(rootHandler, client.CreateHandler(handlers));
        }
    }
}
