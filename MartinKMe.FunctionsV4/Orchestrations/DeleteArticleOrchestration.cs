using MartinKMe.FunctionsV4.Activities;
using MartinKMe.FunctionsV4.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartinKMe.FunctionsV4.Orchestrations
{
    public class DeleteArticleOrchestration
    {
        [FunctionName(nameof(DeleteArticleOrchestration))]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // Get input payload
            ArticleContext articleContext = context.GetInput<ArticleContext>();

            // Delete blob
            await context.CallActivityAsync(nameof(DeleteBlobActivity), articleContext.BlobFileName);

            // Delete article
            await context.CallActivityAsync(nameof(DeleteArticleActivity), articleContext.Article.Key);

            List<string> outputs = new()
            {
                $"Deleted {articleContext.GithubContentApiUri}"
            };

            return outputs;
        }
    }
}
