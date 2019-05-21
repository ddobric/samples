using System;
using System.Threading;

namespace dotnetindocker
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 100;
            while (--n > 0)
            {
                Console.WriteLine("Hello World!");
                Thread.Sleep(1000);
            }
        }
    }
}
