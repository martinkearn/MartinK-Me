using MartinKMe.Domain.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public sealed class GetFileContentsActivity
    {
        private readonly HttpClient client;
        private readonly IUtilityService utilityService;

        public GetFileContentsActivity(HttpClient httpClient, IUtilityService utilityService)// 
        {
            client = httpClient;
            this.utilityService = utilityService;
        }

        [FunctionName(nameof(GetFileContentsActivity))]
        public async Task<string> GetFileContents([ActivityTrigger] string input)
        {
            var response = await client.GetAsync("http://martink.me");
            response.EnsureSuccessStatusCode();
            //var contents = utilityService.Base64Decode(await response.Content.ReadAsStringAsync());
            var contents = await response.Content.ReadAsStringAsync();
            return contents;
        }
    }
}
