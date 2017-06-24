using System;
using System.Collections.Generic;
using System.Text;

namespace Di
{
    public interface ISmsSender
    {
       void Send(string phone, string text);
    }
}
