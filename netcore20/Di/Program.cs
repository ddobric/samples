using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Console;

using System;

namespace Di
{
    class Program
    {
        private static IConfigurationRoot m_Config;

        private static ServiceProvider m_ServiceProvider;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
             .AddJsonFile("mysettings.json", optional: false, reloadOnChange: true);

            m_Config = builder.Build();

            SmsSettings smsSett = new SmsSettings();
            m_Config.GetSection("SmsSettings").Bind(smsSett);

            Func<IServiceProvider, ISmsSender> smsProvFactory = p =>
            {
                MySmsProvider prov = new MySmsProvider(smsSett, p.GetService<ILoggerFactory>().CreateLogger<MySmsProvider>());
                return prov;
            };


            var services = new ServiceCollection();

             m_ServiceProvider = services
             .AddLogging()
             .AddLogging()
             // .AddSingleton(smsProvFactory)
             .AddScoped<ISmsSender>(smsProvFactory)
             .BuildServiceProvider();

            Console.WriteLine("Hello World!");

            var smsSender = m_ServiceProvider.GetService<ISmsSender>();
            smsSender.Send("+4916325673665", "Halli hallo");

            Console.ReadLine();
        }
    }















    //var m_ServiceProvider = services
    //         .AddLogging()
    //         //.Configure<SmsSettings>((o)=> m_Config.GetSection("SmsSettings"))
    //         //.AddSingleton(smsSett)
    //         .AddScoped<ISmsSender>(smsProvFactory)
    //         .BuildServiceProvider();


}
