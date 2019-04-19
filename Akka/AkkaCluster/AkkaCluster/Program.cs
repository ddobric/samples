using Akka.Actor;
using Akka.Configuration;
using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace AkkaCluster
{
    class Program
    {
       
        static void Main(string[] args)
        {
            var assembly = Assembly.Load(new AssemblyName("AkkaShared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("Cluster running...");

            int defaultPort = 8090;

            string config = @"
                akka {
                actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    remote {
                    helios.tcp {
                        port = @PORT
                            public-hostname = localhost
                            hostname = 0.0.0.0
                        }
                }
            }";

            if (args.Length != 0)
            {
                int.TryParse(args[0], out defaultPort);
            }

            config = config.Replace("@PORT", defaultPort.ToString());

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
