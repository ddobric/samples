using System;
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
        }

        protected override void PreStart()
        {
            
        }
    }
}
