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

            sample1();

            //sample2();

            Console.WriteLine("Press any key to continue...");

            Console.ReadLine();
        }

        private static void sample1()
        {
            Task.Run(async () =>
            {
                using (var system = ActorSystem.Create("system"))
                {
                    var actor2 = system.ActorOf(Props.Create(() => new Actor2("Actor2")), "Actor2");

                    var actor1 = system.ActorOf(Props.Create(() => new Actor1("Actor1", actor2)), "Actor1");

                    actor1.Tell(new ReceiveAnDoNothingMsg() { Id = 1});

                    actor2.Tell("Message 1");

                    //actor2.Tell("Message 2");

                    //actor2.Tell("Message 3");

                    //var res = await actor1.Ask(new RequestResponseMsg());
                    //Console.WriteLine($"RESULT: {res}");
                    

                    //actor1.Tell(new CreateChildActor() { NumOfActors = 10 });

                    await Task.Delay(5000);
                }

                Console.ReadLine();

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
                        actors.Add(system.ActorOf(Props.Create(() => new Actor1($"Actor{i}", null)), $"NameActor-ROOT-{i}"));
                    }


                    foreach (var actor in actors)
                    {
                        for (int i = 0; i < 100; i++)
                        {                           
                            bool res = false;

                            while (res == false)
                            {
                                res = await actor.Ask<bool>(new RequestResponseMsg() );
                                if (res == false)
                                    Console.WriteLine($"Message not delivered Message2 - {i}");
                            }
                        }
                    }                   
                }

                Console.ReadLine();
            }).Wait();
        }
    }
}
