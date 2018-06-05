using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Benchmarking
{
    class Program
    {

        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkTests>();

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
