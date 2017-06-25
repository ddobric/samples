using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Di
{
    public class MySmsProvider : ISmsSender
    {
        private readonly SmsSettings m_Settings;
        private readonly ILogger m_Logger;


        public MySmsProvider()
        {

        }

        public MySmsProvider(IOptions<SmsSettings> options, ILogger<MySmsProvider> logger) :this(options.Value, logger)
        {

        }

        public MySmsProvider(SmsSettings settings, ILogger<MySmsProvider> logger)
        {
            m_Settings = settings;
            m_Logger = logger;
        }


        public void Send(string phone, string text)
        {
            m_Logger.LogInformation("Requested to send SMS");
            Console.WriteLine($"to:{phone}, {text}");
            m_Logger.LogInformation($"to:{phone}, {text}");
        }
    }
}
