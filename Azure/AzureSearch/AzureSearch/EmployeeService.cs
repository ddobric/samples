using System;
using System.Collections.Generic;
using System.Text;

namespace AzureSearch
{
    public class WorkItem
    {
        public float Score { get; set; }

        public string ID { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string Description { get; set; }

        public string[] People{ get; set; }
        
    }
}
