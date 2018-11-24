using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Akk1Hello
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Actor Model!");

            //sample1();

            sample2();

            Console.WriteLine("Press any key to continue...");

            Console.ReadLine();
        }

        private static void sample1()
        {
            Task.Run(async () =>
            {
                using (var system = ActorSystem.Create("system"))
                {
                    var actor1 = system.ActorOf(Props.Create(() => new Actor1("Actor1")), "Actor1");

                    var actor2 = system.ActorOf(Props.Create(() => new Actor2("Actor2")), "Actor2");

                    actor1.Tell(new Message1());

                    actor2.Tell(new Message2());

                    var res = await actor1.Ask(new Message2());
                }
            }).Wait();
        }


        private static void sample2()
        {
            Task.Run(async () =>
            {
                using (var system = ActorSystem.Create("system"))
                {
                    List<IActorRef> actors = new List<IActorRef>();

                    for (int i = 0; i < 100; i++)
                    {
                        actors.Add(system.ActorOf(Props.Create(() => new Actor1($"Actor{i}")), $"NameActor-ROOT-{i}"));
                    }


                    foreach (var actor in actors)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            bool res1 = false;
                            bool res2 = false;

                            while (res1 == false)
                            {
                                res1 = await actor.Ask<bool>(new Message1() { Id = i });
                                if (res1 == false)
                                    Console.WriteLine($"Message not delivered Message1 - {i}");
                            }

                            while (res2 == false)
                            {
                                res2 = await actor.Ask<bool>(new Message2() { Id = i });
                                if (res2 == false)
                                    Console.WriteLine($"Message not delivered Message2 - {i}");
                            }
                        }
                    }                   
                }
            }).Wait();
        }
    }
}
