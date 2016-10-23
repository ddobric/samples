using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Azure.AppService.ApiApps.Service;
using System.Dynamic;

namespace DeviceConnectorSample.Controllers
{
    public class DeviceController : ApiController
    {
        /// <summary>
        /// Trigger operation for collecting of telemetry data.
        /// </summary>
        /// <param name="triggerState">mock if you do not have Service Bus connection string. Anything else will
        /// collect telemetry data from service bus.</param>
        /// <param name="sbConnStr">Service Bus connection string. Used only if triggerState not equql 'mock'.</param>
        /// <param name="queuePath">The path of the queue where telemetry data are stored.</param>
        /// <returns>List of telemetry data.</returns>
        [HttpGet]
        [Route("api/device/telemetry")]
        public HttpResponseMessage TelemetryDataPollTrigger(
            string triggerState = null,
            string sbConnStr = null,
            string queuePath = null)
        {

            IDataChannel chn;

            if (triggerState == null)
                chn = chn = new MockChannel();
            else if (triggerState != null && triggerState.ToLower() == "mock")
                chn = new MockChannel();
            else
                chn = new SbChannel(sbConnStr, queuePath);

            var telemetryData = chn.GetTelemetryData();

            if (telemetryData.Count > 0)
            {
                // Extension method provided by the AppService service SDK.
                return this.Request.EventTriggered(new { Data = telemetryData });
            }
            // If there are no files touched after the timestamp, tell the caller to poll again after 1 mintue.
            else
            {
                // Extension method provided by the AppService service SDK.
                return this.Request.EventWaitPoll(new TimeSpan(0, 1, 0));
            }
        }


        public List<string> Get()
        {
            return new string[] { "aaa" }.ToList();
        }
    }
}
