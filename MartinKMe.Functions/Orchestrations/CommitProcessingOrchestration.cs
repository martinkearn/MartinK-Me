using MartinKMe.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Orchestrations
{
    public class CommitProcessingOrchestration
    {
        [FunctionName(nameof(CommitProcessingOrchestration))]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // Get input payload
            var input = context.GetInput<GithubPushWebhookPayload>();

            var outputs = new List<string>();

            var addModifyArticleOrchestrationOutput = await context.CallSubOrchestratorAsync<List<string>>(nameof(AddModifyArticleOrchestration), null);
            outputs.AddRange(addModifyArticleOrchestrationOutput);

            var deleteArticleOrchestrationOutput = await context.CallSubOrchestratorAsync<List<string>>(nameof(DeleteArticleOrchestration), null);
            outputs.AddRange(deleteArticleOrchestrationOutput); 

            return outputs;
        }
    }
}
