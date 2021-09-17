using Markdig;
using MartinKMe.Domain.Interfaces;

namespace MartinKMe.Services
{
    public class MarkdownService : IMarkdownService
    {
        private readonly IUtilityService _utilityService;

        public MarkdownService(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        public string MarkdownToBase64Html(string markdown)
        {
            // parse markdown to html with MarkDig
            var mdPipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseAdvancedExtensions()
                .Build();
            var html = Markdown.ToHtml(markdown, mdPipeline);

            // trim leading <H1> ... Bit hacky as it assumes that the H1 is the first line of html
            var htmlNoH1 = html.Substring(html.IndexOf("</h1>") + 5);

            // Return
            return htmlNoH1;
        }
    }
}
