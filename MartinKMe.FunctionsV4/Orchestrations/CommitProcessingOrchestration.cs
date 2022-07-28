using MartinKMe.Domain.Models;
using MartinKMe.FunctionsV4.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartinKMe.FunctionsV4.Orchestrations
{
    public class CommitProcessingOrchestration
    {
        [FunctionName(nameof(CommitProcessingOrchestration))]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            // Get input payload
            GithubPushWebhookPayload input = context.GetInput<GithubPushWebhookPayload>();

            List<string> outputs = new();

            foreach (Commit commit in input.Commits)
            {
                // Items added in commit
                outputs.AddRange(await CallSubOrchestration(context, commit.Added, nameof(AddModifyArticleOrchestration), commit.Author.Username, input.Repository.Name));

                // Items modified in commit
                outputs.AddRange(await CallSubOrchestration(context, commit.Modified, nameof(AddModifyArticleOrchestration), commit.Author.Username, input.Repository.Name));

                // Items deleted in commit
                outputs.AddRange(await CallSubOrchestration(context, commit.Removed, nameof(DeleteArticleOrchestration), commit.Author.Username, input.Repository.Name));
            }

            // Log and return
            logger.LogInformation("CommitProcessingOrchestration completed for {GithubCommitId}", input.HeadCommit.Id);
            return outputs;
        }

        private static async Task<List<string>> CallSubOrchestration([OrchestrationTrigger] IDurableOrchestrationContext context, List<string> items, string subOrchestration, string author, string repo)
        {
            List<string> outputs = new();

            // Filter the items to only include markdown files in the /blogs/ folder
            List<string> filteredItems = items
                .Where(i => i.EndsWith(".md", StringComparison.InvariantCultureIgnoreCase))
                .Where(i => i.StartsWith("blogs/", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            foreach (string item in filteredItems)
            {
                // Prepare the blob name (.html version of the markdown file)
                StringBuilder blobFileName = new(item.ToLowerInvariant());
                _ = blobFileName.Replace("blogs/", string.Empty);
                _ = blobFileName.Replace(".md", ".html");

                // Prepare the article key (base64 encoded version of the gihub path .. i.e blogs/Test.md)
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(item.ToLowerInvariant());
                string articleKey = Convert.ToBase64String(plainTextBytes);
                Article article = new() { Key = articleKey };

                // Prepare article context
                ArticleContext articleContext = new()
                {
                    GithubContentApiUri = new Uri($"https://api.github.com/repos/{author}/{repo}/contents/{item}"),
                    BlobFileName = blobFileName.ToString().ToLowerInvariant(),
                    Article = article,
                };

                // Call sub-orchestration passing article content
                List<string> subOrchestrationOutput = await context.CallSubOrchestratorAsync<List<string>>(subOrchestration, articleContext);
                outputs.AddRange(subOrchestrationOutput);
            }

            return outputs;
        }
    }
}
