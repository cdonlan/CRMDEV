using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Pfe.Xrm;
using Microsoft.Azure; // Namespace for CloudConfigurationManager 
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Services;
using System.ServiceModel;

namespace CRMIntegration
{
    class Program
    {
        public static Uri address = XrmServiceUriFactory.CreateOnlineOrganizationServiceUri("cdonlanv9", CrmOnlineRegion.NA);
        public static OrganizationServiceManager manager = new OrganizationServiceManager(address, "admin@cdonlanv9.onmicrosoft.com", "pass@word3");
        public static string accountName = "Azure Account 1";
        public static string accountNumber = "12345";

        static void Main(string[] args)
        {

           //AlternateKeyDemo();
            //UpsertDemo();
           // OptimisticConcurrencyDemo();
           // ChangeTrackingDemo();
           ExecuteTransactionDemo();


        }

        public static void ExecuteTransactionDemo()
        {
            Console.Write("Create CRM Org service proxy");
            using (var proxy = manager.GetProxy())
            {
                OrganizationRequestCollection requestCollection = new OrganizationRequestCollection();
                for (int i =0; i < 10; i++)
                {
                    Entity entity = new Entity("account");
                    entity["name"] = "Execute Multiple Record " + i;
                    requestCollection.Add(new CreateRequest { Target = entity });
                }

                ExecuteMultipleRequest request = new ExecuteMultipleRequest()
                {
                    Settings = new ExecuteMultipleSettings()
                    {
                        ContinueOnError = false,
                        ReturnResponses = true

                    },
                    Requests = requestCollection                    
                };

                ExecuteMultipleResponse responseCollection = (ExecuteMultipleResponse) proxy.Execute(request);

            }
        }

        public static void UpsertDemo()
        {
            Console.WriteLine("************** Start Upsert Demo **************");

            Console.WriteLine("Create CRM Org service proxy");
            using (var proxy = manager.GetProxy())
            {
                //user alternate key to get record
                Entity accountToUpdate = new Entity("account", "new_azuretablestorage", accountNumber);
                accountToUpdate["name"] = accountName;
                accountToUpdate["telephone1"] = "444-444-4444";

                UpsertRequest upsertRequest = new UpsertRequest()
                {
                    Target = accountToUpdate
                };

                Console.WriteLine("Execute Upsert Request to update record");
                UpsertResponse upsertResponse = (UpsertResponse)proxy.Execute(upsertRequest);

                if (upsertResponse.RecordCreated)
                    Console.WriteLine("Upsert Response: New Record Created");
                else
                    Console.WriteLine("Upsert Response: Existing Record Updated");


               Entity accountToCreate = new Entity("account", "new_azuretablestorage", "987654321");
                accountToCreate["name"] = "Created via Upsert";
                accountToCreate["telephone1"] = "888-888-8888";
                
                upsertRequest.Target = accountToCreate;

                Console.WriteLine("Execute Upsert Request to create record");
                upsertResponse = (UpsertResponse)proxy.Execute(upsertRequest);

                if (upsertResponse.RecordCreated)
                    Console.WriteLine("Upsert Response: New Record Created");
                else
                    Console.WriteLine("Upsert Response: Existing Record Updated");

            }

            Console.WriteLine("************** End Upsert Demo **************");
            Console.ReadLine();
        }

        public static void OptimisticConcurrencyDemo()
        {
            Console.Write("Create CRM Org service proxy");
            using (var proxy = manager.GetProxy())
            {
                try {
                        Guid accountId;
                        string accountRowVersion;

                    //Create test record
                    Entity testaccount = new Entity("account");
                    testaccount["name"] = "Fourth Coffee";
                    testaccount["creditlimit"] = new Money(50000);

                    accountId = proxy.Create(testaccount);
                    Console.WriteLine("Account '{0}' created with a credit limit of {1}.", testaccount["name"], ((Money)testaccount["creditlimit"]).Value);

                    var account = proxy.Retrieve("account", accountId, new ColumnSet("name", "creditlimit"));
                    Console.WriteLine("\tThe row version of the created account is {0}", account.RowVersion);

                    if (account != null)
                    {
                        // Create an in-memory account object from the retrieved account.
                        Entity updatedAccount = new Entity()
                        {
                            LogicalName = account.LogicalName,
                            Id = account.Id,
                            RowVersion = account.RowVersion
                        };

                        // Update just the credit limit.
                        updatedAccount["creditlimit"] = new Money(1000000);

                        // Set the request's concurrency behavour to check for a row version match.
                        UpdateRequest accountUpdate = new UpdateRequest()
                        {
                            Target = updatedAccount,
                            ConcurrencyBehavior = ConcurrencyBehavior.IfRowVersionMatches
                        };

                        // Do the update.
                        UpdateResponse accountUpdateResponse = (UpdateResponse)proxy.Execute(accountUpdate);
                        Console.WriteLine("Account '{0}' updated with a credit limit of {1}.", account["name"],
                            ((Money)updatedAccount["creditlimit"]).Value);


                        account = proxy.Retrieve("account", updatedAccount.Id, new ColumnSet());
                        Console.WriteLine("\tThe row version of the updated account is {0}", account.RowVersion);
                        accountRowVersion = account.RowVersion;

                        testaccount.Id = account.Id;
                        //old row id....should fail
                        testaccount.RowVersion = updatedAccount.RowVersion;
                        testaccount["name"] = "Forth coffee attempt update";
                        UpdateRequest failedUpdateRequest = new UpdateRequest()
                        {
                            Target = testaccount,
                            ConcurrencyBehavior = ConcurrencyBehavior.IfRowVersionMatches
                        };

                        proxy.Execute(failedUpdateRequest);
                    }
                }
                catch(FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> e)
                {
                    if (e.Detail.ErrorCode == CrmSdk.ErrorCodes.ConcurrencyVersionMismatch)
                    {
                        Console.WriteLine("Concurrency Version Mismatch");
                    }
                    else
                        throw e;
                }
            }
        }

        public static void ChangeTrackingDemo()
        {
            Console.WriteLine("Create CRM Org service proxy");
            using (var proxy = manager.GetProxy())
            {
                List<Entity> initialrecords = new List<Entity>();
                string changetoken = null;

                RetrieveEntityChangesRequest request = new RetrieveEntityChangesRequest();
                request.EntityName = "account";
                request.Columns = new ColumnSet("name");
                request.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1, ReturnTotalRecordCount = false };
               

                while(true)
                {
                    RetrieveEntityChangesResponse response = (RetrieveEntityChangesResponse)proxy.Execute(request);
                    initialrecords.AddRange(response.EntityChanges.Changes.Select(x => (x as NewOrUpdatedItem).NewOrUpdatedEntity).ToArray());
                    Console.WriteLine("Record Changes: " + initialrecords.Count);
                    

                    if(!response.EntityChanges.MoreRecords)
                    {
                        changetoken = response.EntityChanges.DataToken;
                        Console.WriteLine("Current change token: " + changetoken);
                        break;
                    }

                    request.PageInfo.PageNumber++;
                    request.PageInfo.PagingCookie = response.EntityChanges.PagingCookie;
                }

                Entity newAccount = new Entity("account");
                newAccount["name"] = "Change tracking token";
                Console.WriteLine("Create new account");
                proxy.Create(newAccount);

                Console.WriteLine("Create second RetrieveEntityChangeRequest and set current change token");
                RetrieveEntityChangesRequest secondRequest = new RetrieveEntityChangesRequest();
                secondRequest.EntityName = "account";
                secondRequest.Columns = new ColumnSet("name");
                secondRequest.PageInfo = new PagingInfo() { Count = 5000, PageNumber = 1, ReturnTotalRecordCount = false };
                secondRequest.DataVersion = changetoken;

                Console.WriteLine("Execute second RetrieveEntityChangeRequest");
                RetrieveEntityChangesResponse secondResponse = (RetrieveEntityChangesResponse)proxy.Execute(secondRequest);
                List<Entity> changedRecords = new List<Entity>();
                changedRecords.AddRange(secondResponse.EntityChanges.Changes.Select(x => (x as NewOrUpdatedItem).NewOrUpdatedEntity).ToArray());
                Console.WriteLine("Record changes: " + changedRecords.Count);

            }
        }

        public static void AlternateKeyDemo()
        {

            Console.WriteLine("************** Start Alternate Key Demo **************");
            Console.WriteLine("Connect to cloud storage account and create foreign account table");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            Console.WriteLine("Create cloud table client");
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("foreignaccount");
            table.CreateIfNotExists();

            Console.WriteLine("Create CRM Org service proxy");
            using (var proxy = manager.GetProxy())
            {
                Console.WriteLine("Create Azure table entry");
                
                ForeignAccount crmAccount1 = new ForeignAccount(accountNumber, accountName);
                TableOperation insertOperation = TableOperation.Insert(crmAccount1);
                table.Execute(insertOperation);

                Console.WriteLine("Create Account in CRM");
                Entity crmAccount = new Entity("account");
                crmAccount["name"] = accountName;
                crmAccount["new_azuretablestorage"] = accountNumber;
                proxy.Create(crmAccount);

                //new_Altkey
                //new_azuretablestorage
                Console.WriteLine("Access CRM account via alternate key and update phone number");
                Entity updatedAccount = new Entity("account", "new_azuretablestorage", accountNumber);
                
                updatedAccount["telephone1"] = "555-555-5555";
                proxy.Update(updatedAccount);

                Console.WriteLine("************** End **************");
                Console.ReadLine();
            }
        }
    }
}
