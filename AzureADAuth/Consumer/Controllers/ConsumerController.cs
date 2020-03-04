using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Consumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerController : ControllerBase
    {
        private string keyVaultUrl;

        private string tenantId;

        private string clientId;

        private string serviceUrl;

        private string resource;

        private IConfidentialClientApplication app;

        public ConsumerController(IConfiguration configuration)
        {
            keyVaultUrl = configuration["KeyVault:Url"];
            tenantId = configuration["AzureAD:TenantId"];
            clientId = configuration["AzureAD:ClientId"];
            serviceUrl = configuration["Service:BaseUrl"] + "/api/data";
            resource = configuration["Service:AzureAD:AppIdScope"];
        }

        [HttpGet]
        public async Task<ResultDTO> Get()
        {
            var token = await GetTokenForService();
            var data = await GetDataFromService(token);

            return new ResultDTO()
            {
                ConsumerResult = "Data from consumer service",
                UsedToken = token,
                ServiceResult = data
            };
        }

        private async Task<string> GetTokenForService()
        {
            try
            {
                if (app == null)
                {
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var keyVault = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                    var secret = await keyVault.GetSecretAsync(keyVaultUrl, "consumerSecret");

                    app = ConfidentialClientApplicationBuilder.Create(clientId)
                        .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                        .WithClientSecret(secret.Value)
                        .Build();
                }

                var scopes = new string[] { resource };
                var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                return result.AccessToken;
            }
            catch(Exception e)
            {
                return "An error occured while trying to acquire token: " + e;
            }
        }

        private async Task<string> GetDataFromService(string token)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var result = await httpClient.GetAsync(serviceUrl);

            if(result.StatusCode == HttpStatusCode.OK)
            {
                return await result.Content.ReadAsStringAsync();
            }
            else
            {
                return "Error occured while getting data from downstream service. Status code: " + result.StatusCode;
            }
        }
    }

    public class ResultDTO 
    { 
        public string ConsumerResult { get; set; }

        public string UsedToken { get; set; }

        public string ServiceResult { get; set; }
    }
}