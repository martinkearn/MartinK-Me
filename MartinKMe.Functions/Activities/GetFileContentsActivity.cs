using MartinKMe.Domain.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public class GetFileContentsActivity
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IUtilityService utilityService;

        public GetFileContentsActivity(IUtilityService utilityService)//IHttpClientFactory httpClientFactory, 
        {
            //clientFactory = httpClientFactory;
            this.utilityService = utilityService;
        }

        [FunctionName(nameof(GetFileContentsActivity))]
        public async Task<string> GetFileContents([ActivityTrigger] string input)
        {
            //var client = clientFactory.CreateClient();
            //var response = await client.GetAsync(input);
            //response.EnsureSuccessStatusCode();

            //var contents = utilityService.Base64Decode(await response.Content.ReadAsStringAsync());
            var contents = utilityService.Base64Decode("Zm9v");

            return contents;
        }
    }
}
