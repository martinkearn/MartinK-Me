using MartinKMe.Domain.Models;
using MartinKMe.FunctionsV4.Orchestrations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace MartinKMe.FunctionsV4.Clients
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
            string githubRawPayload = await req.Content.ReadAsStringAsync();

            GithubPushWebhookPayload payload;
            try
            {
                payload = JsonConvert.DeserializeObject<GithubPushWebhookPayload>(githubRawPayload);
                log.LogInformation("Received Github webhook for commit {CommitId}", payload.Commits[0].Id);
            }
            catch (JsonReaderException jex)
            {
                string message = "Payload was not in expected format";
                log.LogError(jex, message);
                HttpResponseMessage response = new(System.Net.HttpStatusCode.BadRequest)
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
