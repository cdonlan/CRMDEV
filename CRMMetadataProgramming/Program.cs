using Microsoft.Pfe.Xrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;
using System.IO;
using System.Xml;
using Microsoft.Xrm.Sdk.Metadata.Query;

namespace CRMMetadataProgramming
{
    class Program
    {
        static void Main(string[] args)
        {
            var address = XrmServiceUriFactory.CreateOnlineOrganizationServiceUri("cdonlanv9", CrmOnlineRegion.NA);
            var manager = new OrganizationServiceManager(address, "admin@cdonlanv9.onmicrosoft.com", "pass@word3");

            using (var proxy = manager.GetProxy())
            {
                proxy.EnableProxyTypes();

                RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest()
                {
                    EntityFilters = EntityFilters.Entity,
                    RetrieveAsIfPublished = true
                };
          
                RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)proxy.Execute(request);

                String filename = String.Concat("EntityInfo.xml");
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    // Create Xml Writer.
                    XmlTextWriter metadataWriter = new XmlTextWriter(sw);

                    // Start Xml File.
                    metadataWriter.WriteStartDocument();

                    // Metadata Xml Node.
                    metadataWriter.WriteStartElement("Metadata");

                    foreach (EntityMetadata currentEntity in response.EntityMetadata)
                    {
                        //if (currentEntity.IsIntersect.Value == false)
                        if (true)
                        {
                            // Start Entity Node
                            metadataWriter.WriteStartElement("Entity");

                            // Write the Entity's Information.
                            metadataWriter.WriteElementString("EntitySchemaName", currentEntity.SchemaName);
                            metadataWriter.WriteElementString("OTC", currentEntity.ObjectTypeCode.ToString());
                            metadataWriter.WriteElementString("OwnershipType", currentEntity.OwnershipType.Value.ToString());

                            if (currentEntity.DisplayName.UserLocalizedLabel != null)
                            {
                                metadataWriter.WriteElementString("DisplayName", currentEntity.DisplayName.UserLocalizedLabel.Label);
                                Console.WriteLine("Writing entity: " + currentEntity.DisplayName.UserLocalizedLabel.Label);
                            }

                            if (currentEntity.DisplayCollectionName.UserLocalizedLabel != null)
                                metadataWriter.WriteElementString("DisplayCollectionName", currentEntity.DisplayCollectionName.UserLocalizedLabel.Label);
                            metadataWriter.WriteElementString("IntroducedVersion", currentEntity.IntroducedVersion.ToString());
                            metadataWriter.WriteElementString("AutoRouteToOwnerQueue", currentEntity.AutoRouteToOwnerQueue.ToString());
                            metadataWriter.WriteElementString("CanBeInManyToMany", currentEntity.CanBeInManyToMany.Value.ToString());
                            metadataWriter.WriteElementString("CanBePrimaryEntityInRelationship", currentEntity.CanBePrimaryEntityInRelationship.Value.ToString());
                            metadataWriter.WriteElementString("CanBeRelatedEntityInRelationship", currentEntity.CanBeRelatedEntityInRelationship.Value.ToString());
                            metadataWriter.WriteElementString("CanCreateAttributes", currentEntity.CanCreateAttributes.Value.ToString());
                            metadataWriter.WriteElementString("CanCreateCharts", currentEntity.CanCreateCharts.Value.ToString());
                            metadataWriter.WriteElementString("CanCreateForms", currentEntity.CanCreateForms.Value.ToString());
                            metadataWriter.WriteElementString("CanCreateViews", currentEntity.CanCreateViews.Value.ToString());
                            metadataWriter.WriteElementString("CanModifyAdditionalSettings", currentEntity.CanModifyAdditionalSettings.Value.ToString());
                            metadataWriter.WriteElementString("CanTriggerWorkflow", currentEntity.CanTriggerWorkflow.Value.ToString());

                            metadataWriter.WriteElementString("IsActivity", currentEntity.IsActivity.Value.ToString());
                            //metadataWriter.WriteElementString("ActivityTypeMask", currentEntity.ActivityTypeMask.ToString());

                            metadataWriter.WriteElementString("IsActivityParty", currentEntity.IsActivityParty.Value.ToString());

                            metadataWriter.WriteElementString("IsAuditEnabled", currentEntity.IsAuditEnabled.Value.ToString());
                            metadataWriter.WriteElementString("IsAvailableOffline", currentEntity.IsAvailableOffline.ToString());
                            metadataWriter.WriteElementString("IsChildEntity", currentEntity.IsChildEntity.ToString());
                            metadataWriter.WriteElementString("IsConnectionsEnabled", currentEntity.IsConnectionsEnabled.ManagedPropertyLogicalName.ToString());
                            metadataWriter.WriteElementString("IsCustomEntity", currentEntity.IsCustomEntity.Value.ToString());
                            metadataWriter.WriteElementString("IsCustomizable", currentEntity.IsCustomizable.Value.ToString());

                            metadataWriter.WriteElementString("IsDocumentManagementEnabled", currentEntity.IsDocumentManagementEnabled.Value.ToString());
                            metadataWriter.WriteElementString("IsDuplicateDetectionEnabled", currentEntity.IsDuplicateDetectionEnabled.Value.ToString());
                            if (currentEntity.IsEnabledForCharts != null)
                                metadataWriter.WriteElementString("IsEnabledForCharts", currentEntity.IsEnabledForCharts.Value.ToString());
                            metadataWriter.WriteElementString("IsImportable", currentEntity.IsImportable.Value.ToString());
                            metadataWriter.WriteElementString("IsIntersect", currentEntity.IsIntersect.Value.ToString());

                            metadataWriter.WriteElementString("IsMailMergeEnabled", currentEntity.IsMailMergeEnabled.Value.ToString());
                            metadataWriter.WriteElementString("IsManaged", currentEntity.IsManaged.Value.ToString());
                            metadataWriter.WriteElementString("IsMappable", currentEntity.IsMappable.Value.ToString());

                            metadataWriter.WriteElementString("IsReadingPaneEnabled", currentEntity.IsReadingPaneEnabled.Value.ToString());
                            metadataWriter.WriteElementString("IsRenameable", currentEntity.IsRenameable.Value.ToString());
                            metadataWriter.WriteElementString("IsValidForAdvancedFind", currentEntity.IsValidForAdvancedFind.Value.ToString());
                            metadataWriter.WriteElementString("IsValidForQueue", currentEntity.IsValidForQueue.Value.ToString());
                            metadataWriter.WriteElementString("IsVisibleInMobile", currentEntity.IsVisibleInMobile.Value.ToString());

                            metadataWriter.WriteElementString("PrimaryIdAttribute", currentEntity.PrimaryIdAttribute);
                            metadataWriter.WriteElementString("PrimaryNameAttribute", currentEntity.PrimaryNameAttribute);
                            metadataWriter.WriteElementString("ReportViewName", currentEntity.ReportViewName);
                            metadataWriter.WriteElementString("RecurrenceBaseEntityLogicalName", currentEntity.RecurrenceBaseEntityLogicalName);
                            if (currentEntity.Description.UserLocalizedLabel != null)
                                metadataWriter.WriteElementString("Description", currentEntity.Description.UserLocalizedLabel.Label);



                            // End Entity Node
                            metadataWriter.WriteEndElement();
                        }
                    }

                    // End Metadata Xml Node
                    metadataWriter.WriteEndElement();
                    metadataWriter.WriteEndDocument();

                    // Close xml writer.
                    metadataWriter.Close();

                    Console.WriteLine("Get option sets");
                    RetrieveOptionSetRequest retrieveOptionSetRequest =
                        new RetrieveOptionSetRequest
                        {
                            Name = "connectionrole_category"
                        };

                   Console.WriteLine("Get category option set");

                   RetrieveOptionSetResponse retrieveOptionSetResponse =
                            (RetrieveOptionSetResponse)proxy.Execute(retrieveOptionSetRequest);

                   OptionSetMetadata retrievedOptionSetMetadata =
                     (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;

                    OptionMetadata[] optionList =
                      retrievedOptionSetMetadata.Options.ToArray();


                    foreach(OptionMetadata item in optionList)
                    {
                        Console.WriteLine("Item: " + item.Label.LocalizedLabels[0].Label);
                    }
                }

               Console.WriteLine("Done.");
                Console.ReadLine();
            }
        }
    }
}
