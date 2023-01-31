using MartinKMe.FunctionsV4.Activities;
using MartinKMe.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Text;
using MartinKMe.FunctionsV4.Models;

namespace MartinKMe.FunctionsV4.Orchestrations
{
    public class AddModifyArticleOrchestration
    {
        [FunctionName(nameof(AddModifyArticleOrchestration))]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // Get input payload
            ArticleContext articleContext = context.GetInput<ArticleContext>();

            // Get Github content
            articleContext.GithubContent = await context.CallActivityAsync<GithubContent>(nameof(GetGithubContentActivity), articleContext.GithubContentApiUri);

            // Decode the Base64 contents
            articleContext.PlainContents = (articleContext.GithubContent.Encoding == "base64") ?
                Encoding.UTF8.GetString(Convert.FromBase64String(articleContext.GithubContent.Content)) :
                articleContext.GithubContent.Content;

            // Convert plain Markdown to Html
            articleContext.PlainHtmlContents = await context.CallActivityAsync<string>(nameof(MarkdownToHtmlActivity), articleContext.PlainContents);

            // Upsert html blob to storage
            articleContext.HtmlBlobStorageUri = await context.CallActivityAsync<Uri>(nameof(UpsertBlobActivity), articleContext);

            // Convert Yaml to Article object
            articleContext.Article = await context.CallActivityAsync<Article>(nameof(YamlToMarkdownActivity), articleContext);

            // Upsert Article in table storage
            await context.CallActivityAsync(nameof(UpsertArticleActivity), articleContext.Article);

            List<string> outputs = new()
            {
                $"Added/modified {articleContext.GithubContentApiUri}"
            };

            return outputs;
        }
    }
}
