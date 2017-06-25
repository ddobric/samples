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
#pragma warning disable CS0012 // The type 'Object' is defined in an assembly that is not referenced. You must add a reference to assembly 'netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'.
            Console.WriteLine(MyNetCoreLib2.Go(DateTime.Now.Ticks));
#pragma warning restore CS0012 // The type 'Object' is defined in an assembly that is not referenced. You must add a reference to assembly 'netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'.
        }
    }
}
