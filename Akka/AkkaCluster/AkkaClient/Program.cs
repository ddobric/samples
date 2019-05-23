//#define RUNLOCALLY
using System;
using Akka.Actor;
using Akka.Configuration;
using AkkaShared.Shared;
using Akka.Remote;
using System.Collections.Generic;
using System.Reflection;

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
                                remote = ""akka.tcp://DeployTarget@localhost:8089""
                            }
                        }
                    }
                    remote {
                        helios.tcp {
		                    port = 0
		                    hostname = localhost
                        }
                    }
                }")))
            {


#if RUNLOCALLY
                var remoteAddress = Address.Parse($"akka.tcp://DeployTarget@{host}:8089");

                // Deploy actor to remote process via config
                var remoteActor11 = system.ActorOf(Props.Create(() => new Actor2()), "remoteactor2-config");

                // Deploy actor to remote process via code
                var remoteActor12 =
                    system.ActorOf(
                        Props.Create(() => new Actor2())
                            .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), "remoteactor2-code");
#else

                //var remoteAddress1 = Address.Parse($"akka.tcp://DeployTarget@akka-sum-host1.westeurope.azurecontainer.io:8089");
                //var remoteAddress2 = Address.Parse($"akka.tcp://DeployTarget@akka-sum-host3.westeurope.azurecontainer.io:8089");

                var remoteAddress1 = Address.Parse($"akka.tcp://DeployTarget@localhost:8090");
                var remoteAddress2 = Address.Parse($"akka.tcp://DeployTarget@localhost:8090");

                var remoteActor11 =
                  system.ActorOf(
                      Props.Create(() => new Actor2())
                          .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress1))), "remoteactor21-code");

                var remoteActor12 =
                 system.ActorOf(
                     Props.Create(() => new Actor2())
                         .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress2))), "remoteactor22-code");
#endif
                // Creates the instance of actor
                var actor1 = system.ActorOf(Props.Create(() => new Actor1(new List<IActorRef>() { remoteActor11, remoteActor12 })), "startactor");

                // Runs actor.
                var sum = actor1.Ask(new StartCalcMsg() { SumFrom = 0, SumTo = 10000 }).Result;

                Console.WriteLine($"Calculation ended with sum = {sum}");

                // If you dont have instance of actor, you can search for it.
                var sel = system.ActorSelection("/user/startactor");
                var res = sel.ResolveOne(TimeSpan.FromSeconds(1)).Result;

                // Runs actor
                sum = sel.Ask<double>(
                    new StartCalcMsg()
                    {
                        SumFrom = 0, SumTo = 1000000000
                    }).Result;

                Console.WriteLine($"Calculation ended with sum = {sum}");

                //system.ActorSelection("/user/remoteactor2-config").Tell(new Actor2Message("hi from selection!"));
                //system.ActorSelection("/user/remoteactor2-code").Tell(new Actor2Message("hi from selection!"));

                Console.ReadKey();
            }
        }
    }
}
