using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Pfe.Xrm;

namespace CRMLateBind
{
    class Program
    {
        static void Main(string[] args)
        {
            var address = XrmServiceUriFactory.CreateOnlineOrganizationServiceUri("cdonlanv9", CrmOnlineRegion.NA);
            var manager = new OrganizationServiceManager(address, "admin@cdonlanv9.onmicrosoft.com","pass@word2");

            using (var proxy = manager.GetProxy())
            {
                proxy.EnableProxyTypes();

                Account account = new Account();
                account.Name = "Account Early Bound";
                //Guid id = proxy.Create(account);

                Entity accountLate = new Entity("account");
                accountLate["name"] = "Account Late Bound";
                proxy.Create(accountLate);
            }

        }
    }
}
