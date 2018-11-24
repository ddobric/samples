using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Akk1Hello
{
    public class Actor1 : UntypedActor
    {
        string m_Name;

        public Actor1(string name)
        {
            this.m_Name = name;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Message1 msg1:

                    for (int i = 0; i < 10; i++)
                    {
                        Context.ActorOf(Props.Create(() => new Actor1($"Actor{i}")), $"NameActor-1-{i}{msg1.Id}");
                        Sender.Tell(true);
                    }

                    break;

                case Message2 msg2:
                    for (int i = 0; i < 10; i++)
                    {
                        Context.ActorOf(Props.Create(() => new Actor2($"Actor{i}")), $"NameActor-2-{i}{msg2.Id}");
                        Sender.Tell(true);
                    }
                    break;

            }

            Console.WriteLine($"OnReceive: {Self} - {message}");
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
