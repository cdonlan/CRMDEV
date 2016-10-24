using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace Cdonlanplugin2
{
    public class Class2 : IPlugin
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
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    if(context.OutputParameters.Contains("id"))
                    {
                        if (context.OutputParameters.Contains("id"))
                        {
                            trace.Trace("Target id is " + context.OutputParameters.Contains("id"));

                            Entity followup = new Entity("task");

                            followup["subject"] = "Send e-mail to the new customer.";
                            followup["description"] =
                                "Follow up with the customer. Check if there are any new issues that need resolution.";
                            followup["scheduledstart"] = DateTime.Now.AddDays(7);
                            followup["scheduledend"] = DateTime.Now.AddDays(7);
                            followup["category"] = context.PrimaryEntityName;

                            Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                            string regardingobjectidType = "account";

                            followup["regardingobjectid"] =
                            new EntityReference(regardingobjectidType, regardingobjectid);

                            trace.Trace("FollowupPlugin: Creating the task activity.");
                            orgService.Create(followup);
                        }
                    }
                }
            }
            catch (InvalidPluginExecutionException ex)
            {

            }
        }
    }
}
