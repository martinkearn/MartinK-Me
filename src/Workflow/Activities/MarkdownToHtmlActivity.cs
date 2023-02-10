using Microsoft.Azure.Functions.Worker;

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
        public string RunMarkdownToHtmlActivity([ActivityTrigger] string plainContents, FunctionContext executionContext)
        {
            return _markdownService.MarkdownToHtml(plainContents);
        }
    }
}
