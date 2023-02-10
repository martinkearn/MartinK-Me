using System.Text;
using Microsoft.Azure.Functions.Worker;
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
            articleContext.GithubContent = await context.CallActivityAsync<GithubContent>(nameof(GetGithubContentActivity), articleContext.GithubContentApiUri);

            // Decode the Base64 contents
            articleContext.PlainContents = (articleContext.GithubContent.Encoding == "base64") ?
                Encoding.UTF8.GetString(Convert.FromBase64String(articleContext.GithubContent.Content)) :
                articleContext.GithubContent.Content;

            // Convert plain Markdown to Html
            articleContext.PlainHtmlContents = await context.CallActivityAsync<string>(nameof(MarkdownToHtmlActivity), articleContext.PlainContents);

            // Upsert html blob to storage
            articleContext.HtmlBlobStorageUri = await context.CallActivityAsync<string>(nameof(UpsertBlobActivity), articleContext); //Requires HtmlBlobFileName and PlainContents from ArticleContext

            // Convert Yaml to Article object
            articleContext.Article = await context.CallActivityAsync<Article>(nameof(YamlToMarkdownActivity), articleContext); // Requires PlainContents, HtmlBlobStorageUri, Article from ArticleContext

            // Add other properties to Article
            articleContext.Article.GitHubUrl = articleContext.GithubContent.HtmlUrl;
            articleContext.Article.HtmlBlobFileName = articleContext.HtmlBlobFileName;
            var pathSb = new StringBuilder(string.Join("-", articleContext.Article.Title.Split(Path.GetInvalidFileNameChars())));
            pathSb.Replace(" ", "-");
            articleContext.Article.WebPath = pathSb.ToString().ToLowerInvariant();

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