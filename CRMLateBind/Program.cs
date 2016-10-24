using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Pfe.Xrm;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;


namespace CRMLateBind
{
    class Program
    {
        static void Main(string[] args)
        {
            var address = XrmServiceUriFactory.CreateOnlineOrganizationServiceUri("cdonlanv9", CrmOnlineRegion.NA);
            var manager = new OrganizationServiceManager(address, "admin@cdonlanv9.onmicrosoft.com","pass@word3");

            using (var proxy = manager.GetProxy())
            {
                try {

                    proxy.EnableProxyTypes();

                    Console.WriteLine("Create early bound account");
                    Account account = new Account();
                    account.Name = "Account Early Bound 4";
                    Guid id = proxy.Create(account);

                    Console.WriteLine("Create late bound account");
                    Entity accountLate = new Entity("account");
                    accountLate["name"] = "Account Late Bound 8";
                    Guid id2 = proxy.Create(accountLate);
                    accountLate.Id = id2;

                    UpdateEntityRequest _request = new UpdateEntityRequest();
                    _request.Entity = "account";
                    

                    UpdateEntityResponse _response = (UpdateEntityResponse) proxy.Execute(_request);

                    //Set option set value 
                    /*
                    RetrieveAttributeRequest request = new RetrieveAttributeRequest()
                    {
                        EntityLogicalName = account.LogicalName,
                        LogicalName = "new_category",
                        RetrieveAsIfPublished = true

                    };

                    RetrieveAttributeResponse response = (RetrieveAttributeResponse)proxy.Execute(request);
                    PicklistAttributeMetadata result = (PicklistAttributeMetadata)response.AttributeMetadata;
                    OptionMetadata[] optionList = result.OptionSet.Options.ToArray();

                    accountLate["new_category"] = new OptionSetValue(2);
                   */

                    Console.ReadLine();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }
    }
}
