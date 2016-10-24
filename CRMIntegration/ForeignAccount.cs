using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager 
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table;

namespace CRMIntegration
{
    public class ForeignAccount : TableEntity
    {
        public ForeignAccount(string accountID, string accountName)
        {
            this.PartitionKey = accountID;
            this.RowKey = accountName;
        }

        public ForeignAccount() { }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
