using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;

namespace Console
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main(string[] args)
        {
            try
            {
                System.Console.WriteLine("Console started");
                if (args != null)
                {
                    foreach (var arg in args)
                    {
                        System.Console.WriteLine(arg);
                    }
                }

                // Creating a FabricRuntime connects this host process to the Service Fabric runtime.
                using (FabricRuntime fabricRuntime = FabricRuntime.Create())
                {
                    while (true)
                    {
                        System.Console.WriteLine("Running at {0}", DateTime.Now );
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
