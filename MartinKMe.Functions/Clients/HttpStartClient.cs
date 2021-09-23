using MartinKMe.Domain.Models;
using MartinKMe.Functions.Orchestrations;
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
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Receive payload
            var githubRawPayload = await req.Content.ReadAsStringAsync();

            GithubPushWebhookPayload payload;
            try
            {
                payload = JsonConvert.DeserializeObject<GithubPushWebhookPayload>(githubRawPayload);
                log.LogInformation("Received Github webhook for commit {CommitId}", payload.Commits[0].Id);
            }
            catch (JsonReaderException jex)
            {
                var message = "Payload was not in expected format";
                log.LogError(jex, message);
                HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(message)
                };
                return response;
            }

            // Start CommitProcessingOrchestration
            string instanceId = await starter.StartNewAsync(nameof(CommitProcessingOrchestration), payload);
            log.LogInformation($"Started CommitProcessingOrchestration orchestration with ID = '{instanceId}'.");

            // Return management information payload
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
