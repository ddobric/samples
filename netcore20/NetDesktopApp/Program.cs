using NetCoreLib2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDesktopApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine(MyNetCoreLib2.Go(DateTime.Now.Ticks));
        }
    }
}
