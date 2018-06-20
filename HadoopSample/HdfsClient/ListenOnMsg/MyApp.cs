using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HdfsClient.ListenOnMsg
{
    class MyApp : AISdk
    {
        public override object OnMessge(byte[] input)
        {
            return base.OnMessge(input);
        }
    }

}
