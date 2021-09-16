using MartinKMe.Domain.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public sealed class GetFileContentsActivity
    {
        private readonly HttpClient client;
        private readonly IUtilityService utilityService;

        public GetFileContentsActivity(IHttpClientFactory httpClientFactory, IUtilityService utilityService)// 
        {
            client = httpClientFactory.CreateClient();
            this.utilityService = utilityService;
        }

        [FunctionName(nameof(GetFileContentsActivity))]
        public async Task<string> GetFileContents([ActivityTrigger] string input)
        {
            // Make request to Github
            client.BaseAddress = new Uri(input);
            client.DefaultRequestHeaders.Add("User-Agent", "Martink.me - GetFileContentsActivity");
            var response = await client.GetAsync(input);
            response.EnsureSuccessStatusCode();

            // Read and decode contents
            string rawContents = await response.Content.ReadAsStringAsync();
            dynamic objContents = JsonConvert.DeserializeObject(rawContents);
            var contents = utilityService.Base64Decode((string)objContents.content);

            // Return
            return contents;
        }
    }
}
