using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.Storage.Table;

namespace GuestBook_Data
{
    public class GuestBookEntry :
       TableEntity
    {
        public GuestBookEntry()
        {
            PartitionKey = "1";
         
            MyProperty = new Random().Next();
        }

        public string Message { get; set; }
        
        private string m_GuestName;

        public string GuestName 
        {
            get
            {
                return m_GuestName;
            }
            set
            {
                m_GuestName = value;
            }
        }

        public string PhotoUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public int MyProperty { get; set; }

        public int MyProperty2 { get; set; }
    }
}
