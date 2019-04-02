using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Akk1Hello
{
    public class Actor1 : UntypedActor
    {
        string m_Name;

        IActorRef m_ChildActor;

        List<IActorRef> children = new List<IActorRef>();

        public Actor1(string name, IActorRef childActor = null)
        {
            this.m_Name = name;
            this.m_ChildActor = childActor;
        }

        protected override void OnReceive(object message)
        {
             switch (message)
            {
                case ReceiveAnDoNothingMsg msg1:

                    Console.WriteLine($"Actor1 - OnReceive({nameof(ReceiveAnDoNothingMsg)}: {Self} - [{msg1.Id}] - Sender : {Sender}");

                    break;

                case CreateChildActor msg2:
                    Console.WriteLine($"Actor1 - OnReceive({nameof(CreateChildActor)}: {Self} - [{msg2.NumOfActors}] - Sender : {Sender}");

                    for (int i = 0; i < msg2.NumOfActors; i++)
                    {
                        //children.Add(Context.ActorOf(Props.Create(() => new Actor2($"Actor{i}")), $"NameActor-2-{i}"));
                        Context.ActorOf(Props.Create(() => new Actor2($"Actor{i}")), $"NameActor-2-{i}");
                    }
                    break;

                case RequestResponseMsg msg3:

                    Console.WriteLine($"Actor1 - OnReceive({nameof(CreateChildActor)}: {Self} - [{msg3}] - Sender : {Sender}");

                    Sender.Tell(true, Self);
                 
                    break;

            }
        }

        protected override void PreStart()
        {
            Console.WriteLine($"PreStart: {Self}");
            base.PreStart();
        }

        protected override void PostStop()
        {
            Console.WriteLine($"PostStop: {Self}");
            base.PostStop();
        }
    }
}
