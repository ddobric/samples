using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DeviceConnectorSample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
            name: "TelemetryPollTrigger",
            routeTemplate: "api/device/telemetry/{triggerState}/{rootNamespace}",
                defaults: new { 
                Controller = "Device",
                Action = "TelemetryDataPollTrigger",
                triggerState = RouteParameter.Optional,
                rootNamespace = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
