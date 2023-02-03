using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Workflow.Orchestrations;

namespace Workflow.Clients
{
    public class GithubPushHttpClient
    {
        [Function(nameof(GithubPushHttpClient))]
        public async Task<HttpResponseData> StarGithubPushHttpClient(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            // Receive payload
            string githubRawPayload = new StreamReader(req.Body).ReadToEnd();

            ILogger logger = executionContext.GetLogger(nameof(GithubPushHttpClient));

            GithubPushWebhookPayload payload;
            try
            {
                payload = JsonSerializer.Deserialize<GithubPushWebhookPayload>(githubRawPayload);
                logger.LogInformation("Received Github webhook for commit {CommitId}", payload.Commits[0].Id);
            }
            catch (Exception jex)
            {
                string message = "Payload was not in expected format. Expecting a GithubPushWebhookPayload shaped payload.";
                logger.LogError(jex, message);
                var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                response.WriteString(message);
                return response;
            }

            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(MainOrchestration));
            logger.LogInformation("Created new MainOrchestration with instance ID = {instanceId}", instanceId);

            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}


