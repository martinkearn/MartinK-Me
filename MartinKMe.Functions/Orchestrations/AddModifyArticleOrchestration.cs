using MartinKMe.Functions.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Orchestrations
{
    public class AddModifyArticleOrchestration
    {
        [FunctionName(nameof(AddModifyArticleOrchestration))]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // Get input payload
            var input = context.GetInput<string>();

            // get file contents
            var fileContentsMarkdown = await context.CallActivityAsync<string>(nameof(GetFileContentsActivity), input);

            // Convert markdown to html
            var fileContentsHtml = await context.CallActivityAsync<string>(nameof(MarkdownToHtmlActivity), fileContentsMarkdown);

            var outputs = new List<string>()
            {
                $"Added/modified {input} - {fileContentsHtml}"
            };

            return outputs;
        }
    }
}
