using MartinKMe.Domain.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace MartinKMe.Functions.Activities
{
    public sealed class MarkdownToHtmlActivity
    {
        private readonly IMarkdownService _markdownService;

        public MarkdownToHtmlActivity(IMarkdownService markdownService)
        {
            _markdownService = markdownService;
        }

        [FunctionName(nameof(MarkdownToHtmlActivity))]
        public string MarkdownToHtml([ActivityTrigger] string markdown)
        {
            // Convert markdown to html
            var fileContents = _markdownService.MarkdownToBase64Html(markdown);

            // Return
            return fileContents;
        }
    }
}
