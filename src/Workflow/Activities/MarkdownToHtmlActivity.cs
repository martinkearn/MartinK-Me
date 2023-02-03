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

        [FunctionName(nameof(MarkdownToHtmlActivity))]
        public string RunMarkdownToHtmlActivity([ActivityTrigger] string markdown, FunctionContext executionContext)
        {
            return _markdownService.MarkdownToHtml(markdown);
        }
    }
}
