using System;
using System.Threading;

namespace myjob
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 50;
            while (--n > 0)
            {
                Console.WriteLine($"{n}");
                Thread.Sleep(1000);
            }
        }
    }
}
