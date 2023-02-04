using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

namespace Workflow.Activities
{
    public sealed class MarkdownToHtmlActivity
    {
        private readonly IMarkdownService _markdownService;

        public MarkdownToHtmlActivity(IMarkdownService markdownService)
        {
            _markdownService = markdownService;
        }

        [Function(nameof(MarkdownToHtmlActivity))]
        public string RunMarkdownToHtmlActivity([ActivityTrigger] ArticleContext articleContext, FunctionContext executionContext)
        {
            return _markdownService.MarkdownToHtml(articleContext.PlainContents);
        }
    }
}
