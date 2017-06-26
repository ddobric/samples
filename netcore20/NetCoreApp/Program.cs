
using NetDesktopLib;
using System;

namespace NetCoreApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!. Press any key to exit...");
            
            callNetCoreLib();

            callNetDesktopLib();

            Console.ReadLine();
        }

        /// <summary>
        /// Invokes "Library" built with .NET Desktop frmework
        /// </summary>
        private static void callNetDesktopLib()
        {
            Console.WriteLine(MyNetDesktopLib.Go(DateTime.Now.Ticks));
        }


        /// <summary>
        /// Invokes "Library" build with lower .netstandard
        /// </summary>
        private static void callNetCoreLib()
        {
            Console.WriteLine(NetCoreLib.NetCoreLib.Go(7));
        }
    }
}
