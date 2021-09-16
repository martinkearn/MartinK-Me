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

            // Items added in commit
            outputs.AddRange(await CallSubOrchestration(context, input.HeadCommit.Added, nameof(AddModifyArticleOrchestration), input.HeadCommit.Author.Username, input.Repository.Name));

            // Items modified in commit
            outputs.AddRange(await CallSubOrchestration(context, input.HeadCommit.Modified, nameof(AddModifyArticleOrchestration), input.HeadCommit.Author.Username, input.Repository.Name));

            // Items deleted in commit
            outputs.AddRange(await CallSubOrchestration(context, input.HeadCommit.Removed, nameof(DeleteArticleOrchestration), input.HeadCommit.Author.Username, input.Repository.Name));

            return outputs;
        }

        private static async Task<List<string>> CallSubOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context, List<string> items, string subOrchestration, string author, string repo)
        {
            var outputs = new List<string>();

            foreach (var item in items)
            {
                // Check the commit is a markdown file and in the right path in the repo
                if (item.ToLowerInvariant().EndsWith(".md") && item.ToLowerInvariant().StartsWith("blogs/"))
                {
                    // Prepare Github API url for item
                    var itemGitHubApiUrl = $"https://api.github.com/repos/{author}/{repo}/contents/{item}";

                    // Call sub-orchestration
                    var subOrchestrationOutput = await context.CallSubOrchestratorAsync<List<string>>(subOrchestration, itemGitHubApiUrl);
                    outputs.AddRange(subOrchestrationOutput);
                }
            }

            return outputs;
        }


    }
}
