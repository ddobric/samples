using System;
using System.Threading;
using Akka.Actor;

namespace AkkaShared.Shared
{
    /// <summary>
    /// Actor that just replies the message that it received earlier
    /// </summary>
    public class Actor2 : ReceiveActor
    {
        public Actor2()
        {


            Receive<StartCalcMsg>(msg =>
            {
                double sum = 0;
                Console.WriteLine($"{nameof(Actor2)} = {Self}- {Sender}: From: {msg.SumFrom}, To: {msg.SumTo}");

                for (ulong i = msg.SumFrom; i < msg.SumTo; i++)
                {
                    //Console.SetCursorPosition(10, 10);
                    //Console.Write(Text);
                    sum += i;
                }

                Console.WriteLine($"{Self} - Sum = {sum}");

                Sender.Tell(sum, Self);
            });

            Receive<LongRunningMsg>(msg =>
            {
                Console.WriteLine($"{Self.Path} - start");

                ulong sum = 0;
                for (ulong i =0; i < 1000000000; i++)
                {
                    //Console.SetCursorPosition(10, 10);
                    //Console.Write(Text);
                    sum += i;
                }

                Console.WriteLine($"{Self.Path} - done");

                Sender.Tell(0, Self);
            });

            Receive<SingleLongRunningMsg>(msg =>
            {
                Console.WriteLine($"{Self.Path} - start");

                Thread.Sleep(1000 * 60 * 5);

                Sender.Tell(0, Self);
            });

            
        }

        protected override void Unhandled(object message)
        {
            Console.WriteLine(message);
            base.Unhandled(message);
        }

        protected override void PreStart()
        {

        }
    }
}
