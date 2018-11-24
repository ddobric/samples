using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Akk1Hello
{
    public class Actor2 : UntypedActor
    {
        string m_Name;

        public Actor2(string name)
        {
            this.m_Name = name;
        }
        
        protected override void OnReceive(object message)
        {
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
