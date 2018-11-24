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
                    break;

                case Message2 msg2:
                    Sender.Tell(new { a = 1, b = 2 });
                    break;

            }

            Console.WriteLine($"OnReceive: {nameof(Actor1)} - {message}");
        }

        protected override void PreStart()
        {
            Console.WriteLine($"PreStart: {nameof(Actor1)}");
            base.PreStart();
        }

        protected override void PostStop()
        {
            Console.WriteLine($"PostStop: {nameof(Actor1)}");
            base.PostStop();
        }
    }
}
