using Markdig;

namespace Workflow.Services
{
    /// <inheritdoc/>
    public class MarkdownService : IMarkdownService
    {
        public string MarkdownToHtml(string markdown)
        {
            // Parse markdown to html with MarkDig
            var mdPipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseAdvancedExtensions()
                .Build();
            var html = Markdown.ToHtml(markdown, mdPipeline);

            // Return
            return html;
        }
    }
}
