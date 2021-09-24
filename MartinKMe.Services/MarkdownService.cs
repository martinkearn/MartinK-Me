using Markdig;
using MartinKMe.Domain.Interfaces;

namespace MartinKMe.Services
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

            // Trim leading <H1> ... Bit hacky as it assumes that the H1 is the first line of html
            var htmlNoH1 = html.Substring(html.IndexOf("</h1>") + 5);

            // Return
            return htmlNoH1;
        }
    }
}
