using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace NetCoreGpio
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create SAS token sample.
            string token = CreateToken("daenethub.azure-devices.net/devices/pidevice", "iothubowner", "ylxkhcFOU8lWLTIG6W75vnEQpdG97corpQ5JUbNytSw=", 1000*86400);

            // Controlling hardware with GPIO
            //GpioSample.RunAync(args).Wait();

            // REading GPIO and sending telemetry to IoTHub
            //TelemetrySample.RunAync(args).Wait();

            // Invoking REST methods hosted on PI.
            WebMethods.RunAync(args).Wait();
        }

        private static string CreateToken(string resourceUri, string policyName, string key, int expiryInSeconds = 36000)
        {
            TimeSpan fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
            string expiry = Convert.ToString((int)fromEpochStart.TotalSeconds + expiryInSeconds);

            string stringToSign = WebUtility.UrlEncode(resourceUri) + "\n" + expiry;

            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key));
            string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            string token = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}", WebUtility.UrlEncode(resourceUri), WebUtility.UrlEncode(signature), expiry);

            if (!String.IsNullOrEmpty(policyName))
            {
                token += "&skn=" + policyName;
            }

            return token;
        }

        internal static string ConnStr = "todo";
    }


}


