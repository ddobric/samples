using Akka.Actor;
using AkkaShared.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AkkaClient
{
    class Actor1 : ReceiveActor
    {
        private List<IActorRef> m_RemoteActors;

        public Actor1(List<IActorRef> remoteActors)
        {
            m_RemoteActors = remoteActors;

            foreach (var remoteActor in m_RemoteActors)
            {
                Context.Watch(remoteActor);
            }

            Receive<StartCalcMsg>(msg =>
           {
               Console.WriteLine($"{nameof(Actor1)} -  Receive<Actor1Message> - {msg} - Calculation started.");

               int nodeSubstract = 0;
               ulong perPartition = (msg.SumTo - msg.SumFrom) / (ulong)m_RemoteActors.Count;
               ulong rest = msg.SumTo % (ulong)m_RemoteActors.Count;
               if (rest > 0)
               {
                   perPartition++;
                   nodeSubstract = 1;
               }

               Task<double>[] tasks = new Task<double>[m_RemoteActors.Count];

               for (int i = 0; i < m_RemoteActors.Count - nodeSubstract; i++)
               {
                   tasks[i] = m_RemoteActors[i].Ask<double>(new StartCalcMsg() { SumFrom = (ulong)i * perPartition, SumTo = ((ulong)i + 1) * perPartition });
               }

               if (rest > 0)
               {
                   tasks[m_RemoteActors.Count - 1] = m_RemoteActors[m_RemoteActors.Count - 1].Ask<double>(new StartCalcMsg()
                   { SumFrom = perPartition * (ulong)m_RemoteActors.Count - 1, SumTo = msg.SumTo });
               }

               Task.WaitAll(tasks);

               double sum = 0;
               foreach (var task in tasks)
               {
                   sum += task.Result;
               }

               Sender.Tell(sum, Self);
           });

            Receive<Terminated>(terminated =>
            {
                Console.WriteLine($"{nameof(Actor1)} termintion - {terminated.ActorRef}");
                Console.WriteLine("Was address terminated? {0}", terminated.AddressTerminated);
            });
        }

        protected override void PreStart()
        {
            Console.WriteLine("Actor1 started.");
            //m_HelloTask = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
            //    TimeSpan.FromSeconds(1), Context.Self, new Actor1Message(), ActorRefs.NoSender);
        }

        protected override void PostStop()
        {
            Console.WriteLine("Actor1 stoped.");
        }
    }
}
