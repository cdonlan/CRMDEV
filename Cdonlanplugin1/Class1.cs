using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;


namespace Cdonlanplugin1
{
    public class Plugin1 : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService orgService = serviceFactory.CreateOrganizationService(context.UserId);

           
            trace.Trace("Fired");

            try
            {
                
                // check target is an entity
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];

                    trace.Trace("Target contains " + entity.Attributes.Count.ToString() + " attributes");


                    //Try to get fax field
                    //if (entity.Attributes.Contains("fax") && entity.Attributes["fax"] != null)
                    //    trace.Trace("Target contains Fax Attribute: " + entity.Attributes["fax"]);
                    //else
                    //    trace.Trace("Target doesnt contain Fax attribute"); 

                    Entity preEntity = (Entity)context.PreEntityImages["AccountUpdate"];
                    if (preEntity != null)
                        trace.Trace("Pre entity contains " + preEntity.Attributes.Count.ToString() +" attribute");

                    Entity postEntity = (Entity)context.PostEntityImages["AccountUpdate"];
                    if (postEntity != null)
                        trace.Trace("Post entity contains " + postEntity.Attributes.Count.ToString() + " attributes");



                    //if (postEntity.Attributes.Contains("fax") && preEntity.Attributes.Contains("fax"))
                    //{
                    //    string foo = postEntity.Attributes["fax"].ToString();
                    //    trace.Trace("pre entity fax is: " + preEntity.Attributes["fax"].ToString());
                    //    trace.Trace("pre entity fax is: " + postEntity.Attributes["fax"].ToString());
                    //}
                    //else
                    //{
                    //    trace.Trace("entity doesnt contain fax field");

                    //}

                    try
                    {
                        //Update entity description
                       entity["description"] = "Updated in plugin";
                       trace.Trace("updated description");

                        //if (context.OutputParameters.Contains("id"))
                        //{
                        //    trace.Trace("Target id is " + context.OutputParameters.Contains("id"));

                          
                        //}
                        //else
                        //{
                        //    trace.Trace("Target id is NULL");
                        //}


                    }
                    catch(FaultException<OrganizationServiceFault> ex)
                    {
                        trace.Trace("Create new activity");
                    }
                }
            }
            catch(InvalidPluginExecutionException ex)
            {
                throw new InvalidPluginExecutionException("There was an issue with the plug:" + ex.Message);
            }
            finally
            {
                trace.Trace("End of plugin");
            }

        }
     }
}
