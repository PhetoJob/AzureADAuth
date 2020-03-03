using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Consumer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerController : ControllerBase
    {
        private string serviceUrl;

        private string resource;

        private IConfidentialClientApplication app;

        public ConsumerController(IConfiguration configuration)
        {
            serviceUrl = configuration["Service:BaseUrl"] + "/api/data";
            resource = configuration["Service:AzureAD:AppIdURI"];
            app = ConfidentialClientApplicationBuilder.Create(configuration["AzureAD:ClientId"])
                .WithAuthority(AzureCloudInstance.AzurePublic, configuration["AzureAD:TenantId"])
                .WithClientSecret(configuration["AzureAD:ClientSecret"])
                .Build();
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
            var scopes = new string[] { resource };
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
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