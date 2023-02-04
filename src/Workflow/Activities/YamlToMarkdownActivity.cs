using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

namespace Workflow.Activities
{
    public sealed class YamlToMarkdownActivity
    {
        private readonly IYamlService _yamlService;

        public YamlToMarkdownActivity(IYamlService yamlService)
        {
            _yamlService = yamlService;
        }

        [Function(nameof(YamlToMarkdownActivity))]
        public Article RunYamlToMarkdownActivity([ActivityTrigger] ArticleContext articleContext, FunctionContext executionContext)
        {
            // Convert yaml to article
            return _yamlService.YamlToArticle(articleContext.PlainContents, articleContext.HtmlBlobStorageUri, articleContext.Article);
        }
    }
}
