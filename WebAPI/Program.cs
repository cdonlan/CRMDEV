using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Security;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace WebAPI
{
    class Program
    {

        public static string token;

        static void Main(string[] args)
        {

            //string clientId = "2cffaf60-f880-4142-b4b7-ace5cfb3916c";
            string clientId = "c14d7af3-7b54-486a-a119-9b09f5caf236";
            string redirectUrl = "https://cmdonlan.crm.dynamics.com/foo";
            string resource = "https://cmdonlan.crm.dynamics.com/";

            const string _username = "admin@cmdonlan.onmicrosoft.com";
            const string _password = "pass@word1";
            UserCredential creds = new UserCredential(_username, _password);

            AuthenticationContext authContext = new AuthenticationContext("https://login.windows.net/common", false);
            //AuthenticationResult result = authContext.AcquireToken(resource, clientId, new Uri(redirectUrl));
            AuthenticationResult result2 = authContext.AcquireToken(resource, clientId, creds);

            token = result2.AccessToken.ToString();

            Task.WaitAll(Task.Run(async () => await DoWork()));

        }

        private static async Task DoWork()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://cmdonlan.crm.dynamics.com/");
                httpClient.Timeout = new TimeSpan(0, 2, 0);
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //Get Accounts
                HttpResponseMessage whoAmIResponse =
                    await httpClient.GetAsync("/api/data/v8.2/accounts");
               
                if (whoAmIResponse.IsSuccessStatusCode)
                {

                    JsonTextReader reader = new JsonTextReader(new StringReader( whoAmIResponse.Content.ReadAsStringAsync().Result));
                    while (reader.Read())
                        {
                            if (reader.Value != null)
                                {
                                    Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                                }
                            else
                            { 
                                    Console.WriteLine("Token: {0}", reader.TokenType);
                                }
                        }

                    Console.ReadLine();
                }
                else
                    return;

            }
        }
    }
}
