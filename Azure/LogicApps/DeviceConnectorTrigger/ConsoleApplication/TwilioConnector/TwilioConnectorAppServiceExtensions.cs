using System;
using System.Net.Http;
using Microsoft.Azure.AppService;

namespace ConsoleApplication
{
    public static class TwilioConnectorAppServiceExtensions
    {
        public static TwilioConnector CreateTwilioConnector(this IAppServiceClient client)
        {
            return new TwilioConnector(client.CreateHandler());
        }

        public static TwilioConnector CreateTwilioConnector(this IAppServiceClient client, params DelegatingHandler[] handlers)
        {
            return new TwilioConnector(client.CreateHandler(handlers));
        }

        public static TwilioConnector CreateTwilioConnector(this IAppServiceClient client, Uri uri, params DelegatingHandler[] handlers)
        {
            return new TwilioConnector(uri, client.CreateHandler(handlers));
        }

        public static TwilioConnector CreateTwilioConnector(this IAppServiceClient client, HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
        {
            return new TwilioConnector(rootHandler, client.CreateHandler(handlers));
        }
    }
}
