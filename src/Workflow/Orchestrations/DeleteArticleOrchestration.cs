using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace Workflow.Orchestrations
{
    public class DeleteArticleOrchestration
    {
        [Function(nameof(DeleteArticleOrchestration))]
        public async Task<List<string>> RunDeleteArticleOrchestration([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            // Get input payload
            ArticleContext articleContext = context.GetInput<ArticleContext>();

            // Delete blob
            await context.CallActivityAsync(nameof(DeleteBlobActivity), articleContext.HtmlBlobFileName);

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

