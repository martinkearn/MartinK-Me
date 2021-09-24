using MartinKMe.Domain.Models;
using MartinKMe.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            // Filter the items to only include markdown files in the /blogs/ folder
            var filteredItems = items
                .Where(i => i.EndsWith(".md", StringComparison.InvariantCultureIgnoreCase))
                .Where(i => i.StartsWith("blogs/", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            foreach (var item in filteredItems)
            {
                // Prepare the blob name (.html version of the markdown file)
                var blobFileName = new StringBuilder(item.ToLowerInvariant());
                blobFileName.Replace("blogs/", string.Empty);
                blobFileName.Replace(".md", ".html");

                // Prepare the article key (base64 encoded version of the gihub path .. i.e blogs/Test.md)
                var plainTextBytes = Encoding.UTF8.GetBytes(item.ToLowerInvariant());
                var articleKey = Convert.ToBase64String(plainTextBytes);

                // Prepare article context
                var articleContext = new ArticleContext()
                {
                    GithubContentApiUri = new Uri($"https://api.github.com/repos/{author}/{repo}/contents/{item}"),
                    BlobFileName = blobFileName.ToString().ToLowerInvariant(),
                    ArticleKey = articleKey,
                };

                // Call sub-orchestration passing article content
                var subOrchestrationOutput = await context.CallSubOrchestratorAsync<List<string>>(subOrchestration, articleContext);
                outputs.AddRange(subOrchestrationOutput);
            }

            return outputs;
        }
    }
}
