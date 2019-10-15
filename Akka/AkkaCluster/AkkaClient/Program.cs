#define RUNLOCALLY
using System;
using Akka.Actor;
using Akka.Configuration;
using AkkaShared.Shared;
using Akka.Remote;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AkkaClient.Deployer
{
    class Program
    {

        static void Main(string[] args)
        {
#if RUNLOCALLY
            string host = "localhost";
#else
            string host = "akka-sum-host1.westeurope.azurecontainer.io";
#endif


            //var r = typeof(Actor2).Assembly.FullName;
            //var assembly = Assembly.Load(new AssemblyName("AkkaShared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("Client running...");

            using (var system = ActorSystem.Create("Deployer", ConfigurationFactory.ParseString(@"
                akka {  
                    actor{
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        deployment {
                            /remoteactor2-config {
                                remote = ""akka.tcp://DeployTarget@localhost:8090""
                            }
                        }
                    }
                    remote {
                        dot-netty.tcp {
		                    port = 8080
		                    hostname = localhost
                        }
                    }
                }")))
            {


#if RUNLOCALLY
                var remoteAddress = Address.Parse($"akka.tcp://DeployTarget@{host}:8090");

                //// Deploy actor to remote process via config
                //var remoteActor11 = system.ActorOf(Props.Create(() => new Actor2()), "remoteactor2-config");

                //// Deploy actor to remote process via code
                //var remoteActor12 =
                //    system.ActorOf(
                //        Props.Create(() => new Actor2())
                //            .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), "remoteactor2-code");


               StartLongRunningMultiActors(system, remoteAddress);
               // StartLongRunningSingleActor(system, remoteAddress);
                return;
#else

                var remoteAddress1 = Address.Parse($"akka.tcp://DeployTarget@akka-sum-host1.westeurope.azurecontainer.io:8089");
                var remoteAddress2 = Address.Parse($"akka.tcp://DeployTarget@akka-sum-host3.westeurope.azurecontainer.io:8089");

                //var remoteAddress1 = Address.Parse($"akka.tcp://DeployTarget@localhost:8090");
                //var remoteAddress2 = Address.Parse($"akka.tcp://DeployTarget@localhost:8091");
                Console.ReadLine();

                var remoteActor11 =
                  system.ActorOf(
                      Props.Create(() => new Actor2())
                          .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress1))), "remoteactor21-code");

                var remoteActor12 =
                 system.ActorOf(
                     Props.Create(() => new Actor2())
                         .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress2))), "remoteactor22-code");

                 // Runs actor.
                var sum = actor1.Ask(new StartCalcMsg() { SumFrom = 0, SumTo = 10000 }).Result;

                Console.WriteLine($"Calculation ended with sum = {sum}");

                // Creates the instance of actor
                var actor1 = system.ActorOf(Props.Create(() => new Actor1(new List<IActorRef>() { remoteActor11, remoteActor12 })), "startactor");
            
#endif

                // If you dont have instance of actor, you can search for it.
                var sel = system.ActorSelection("/user/startactor");
                var res = sel.ResolveOne(TimeSpan.FromSeconds(1)).Result;

                // Runs actor
                var sum = sel.Ask<double>(
                    new StartCalcMsg()
                    {
                        SumFrom = 0,
                        SumTo = 1000000000
                    }).Result;

                Console.WriteLine($"Calculation ended with sum = {sum}");

                //system.ActorSelection("/user/remoteactor2-config").Tell(new Actor2Message("hi from selection!"));
                //system.ActorSelection("/user/remoteactor2-code").Tell(new Actor2Message("hi from selection!"));

                Console.ReadKey();
            }
        }

        private static void StartLongRunningMultiActors(ActorSystem system, Address remoteAddress)
        {

            ParallelOptions opts = new ParallelOptions();
            opts.MaxDegreeOfParallelism = 100;

            Parallel.For(0, 256, opts, (indx) =>
            {
                Console.WriteLine($"START: {indx}");

                var act = system.ActorOf(
               Props.Create(() => new Actor2())
                   .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), $"long-running-actor-{Guid.NewGuid()}-{indx}");

                while (true)
                {
                    try
                    {
                        int res = act.Ask<int>(new LongRunningMsg(), TimeSpan.FromMinutes(3)).Result;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Err: {indx}. Retry...");
                    }
                }

                Console.WriteLine($"END: {indx}");
            });

            Console.WriteLine($"COMPLETED!!!");
        }

        private static void StartLongRunningSingleActor(ActorSystem system, Address remoteAddress)
        {

            ParallelOptions opts = new ParallelOptions();
            opts.MaxDegreeOfParallelism = 100;

            Console.WriteLine($"START...");

            var act = system.ActorOf(
           Props.Create(() => new Actor2())
               .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), $"long-running-actor");

            while (true)
            {
                try
                {
                    int res = act.Ask<int>(new SingleLongRunningMsg(), TimeSpan.FromMinutes(6)).Result;
                   
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Err: Retry...");
                }
            }

        
            Console.WriteLine($"COMPLETED!!!");
        }
    }
}
