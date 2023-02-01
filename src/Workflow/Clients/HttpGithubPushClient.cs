using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MK.Functions
{
    public class HttpGithubPushClient
    {
        [FunctionName(nameof(HttpGithubPushClient))]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Receive payload
            string rawPayload = await req.Content.ReadAsStringAsync();

            var payload = JsonSerializer.Deserialize<Dictionary<string, string>>(rawPayload); // Expects {"foo":"bar"}

            // Start CommitProcessingOrchestration
            string instanceId = await starter.StartNewAsync("workflow", payload);
            log.LogInformation($"Started CommitProcessingOrchestration orchestration with ID = '{instanceId}'.");

            // Return management information payload
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}