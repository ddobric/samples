using Akka.Actor;
using Akka.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;


namespace AkkaCluster
{
    class Program
    {
       /// <summary>
       /// --AKKAPORT 8089 --AKKAPUBLICHOST localhost --AKKASEEDHOST localhost:8089
       /// </summary>
       /// <param name="args"></param>
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables();
            builder.AddCommandLine(args);

            IConfigurationRoot netConfig = builder.Build();

            var assembly = Assembly.Load(new AssemblyName("AkkaShared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("Cluster running...");

            int port = 8089;
            string publicHostname = "localhost";
            string seedhostsStr = String.Empty;

            if (netConfig["AKKAPORT"] != null)
                int.TryParse(netConfig["AKKAPORT"], out port);

            if (netConfig["AKKAPUBLICHOST"] != null)
                publicHostname = netConfig["AKKAPUBLICHOST"];

            if (netConfig["AKKASEEDHOSTS"] != null)
            {
                seedhostsStr = netConfig["AKKASEEDHOSTS"];
            }

            string config = @"
                akka {
                    actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    remote {
                        helios.tcp {
                            port = @PORT
                                public-hostname = @PUBLICHOSTNAME
                                hostname = 0.0.0.0
                            }
                    }
                    cluster {
                        seed-nodes = [@SEEDHOST]
                    }
            }";
         
            config = config.Replace("@PORT", port.ToString());
            config = config.Replace("@PUBLICHOSTNAME", publicHostname);
            
            if (seedhostsStr.Length > 0)
            {
                var seedHosts = seedhostsStr.Split(',');
                seedHosts = seedHosts.Select(h => h.TrimStart(' ').TrimEnd(' ')).ToArray();
                StringBuilder sb = new StringBuilder();
                bool isFirst = true;

                foreach (var item in seedHosts)
                {
                    if (isFirst == false)
                        sb.Append(", ");

                    sb.Append($"\"akka.tcp://DeployTarget@{item}\"");
                    //example: seed - nodes = ["akka.tcp://ClusterSystem@localhost:8081"]

                    isFirst = false;
                }

                config = config.Replace("@SEEDHOST", sb.ToString());
            }
            else
                config = config.Replace("@SEEDHOST", String.Empty);

            Console.WriteLine(config);

            using (var system = ActorSystem.Create("DeployTarget", ConfigurationFactory.ParseString(config)))
            {
                var cts = new CancellationTokenSource();
                AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
                Console.CancelKeyPress += (sender, cpe) =>
                {
                    CoordinatedShutdown.Get(system).Run(reason: CoordinatedShutdown.ClrExitReason.Instance).Wait();
                    cts.Cancel();
                };

                system.WhenTerminated.Wait();

                return;
            }
        }

        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }
    }
}
