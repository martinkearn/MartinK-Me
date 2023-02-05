using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.DurableTask;

namespace Workflow.Orchestrations
{
    public class AddModifyArticleOrchestration
    {
        [Function(nameof(AddModifyArticleOrchestration))]
        public async Task<List<string>> RunAddModifyArticleOrchestration([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            // Get input payload
            ArticleContext articleContext = context.GetInput<ArticleContext>();

            // Get Github content
            articleContext.GithubContent = await context.CallActivityAsync<GithubContent>(nameof(GetGithubContentActivity), articleContext);

            // Decode the Base64 contents
            articleContext.PlainContents = (articleContext.GithubContent.Encoding == "base64") ?
                Encoding.UTF8.GetString(Convert.FromBase64String(articleContext.GithubContent.Content)) :
                articleContext.GithubContent.Content;

            // Convert plain Markdown to Html
            articleContext.PlainHtmlContents = await context.CallActivityAsync<string>(nameof(MarkdownToHtmlActivity), articleContext);

            // Upsert html blob to storage
            articleContext.HtmlBlobStorageUri = await context.CallActivityAsync<string>(nameof(UpsertBlobActivity), articleContext);

            // Convert Yaml to Article object
            articleContext.Article = await context.CallActivityAsync<Article>(nameof(YamlToMarkdownActivity), articleContext);

            // Upsert Article in table storage
            await context.CallActivityAsync(nameof(UpsertArticleActivity), articleContext);

            List<string> outputs = new()
            {
                $"Added/modified {articleContext.GithubContentApiUri}"
            };

            return outputs;
        }
    }
}