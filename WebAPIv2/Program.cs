using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Samples.HelperCode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;


namespace WebAPIv2
{
    class Program
    {
        private HttpClient httpClient;

        static void Main(string[] args)
        {
            Co
        }

        private void ConnectToCRM(String[] cmdargs)
        {
            Configuration config = null;
            if (cmdargs.Length > 0)
                config = new FileConfiguration(cmdargs[0]);
            else
                config = new FileConfiguration(null);
            Authentication auth = new Authentication(config);
            httpClient = new HttpClient(auth.ClientHandler, true);
            httpClient.BaseAddress = new Uri(config.ServiceUrl + "api/data/v8.1/");
            httpClient.Timeout = new TimeSpan(0, 2, 0);
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
