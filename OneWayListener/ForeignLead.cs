using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table;

namespace OneWayListener
{
    class ForeignLead : TableEntity
    {
        public ForeignLead(string id, string fullname)
        {
            this.PartitionKey = id;
            this.RowKey = fullname;

        }

        public ForeignLead() { }


        public string ID { get; set; }

        public string Fullname { get; set; }
    }
}
