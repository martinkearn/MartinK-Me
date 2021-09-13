using MartinKMe.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Clients
{
    public class HttpStartClient
    {
        [FunctionName(nameof(HttpStartClient))]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Receive payload
            var githubRawPayload = await req.Content.ReadAsStringAsync();
            var payload = JsonConvert.DeserializeObject<GithubPushWebhookPayload>(githubRawPayload);

            // Function input comes from the request content.
            //string instanceId = await starter.StartNewAsync("Function1", null);

            //log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, "noid");
        }
    }
}
