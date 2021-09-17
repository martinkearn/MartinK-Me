using MartinKMe.Functions.Activities;
using MartinKMe.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MartinKMe.Functions.Models;

namespace MartinKMe.Functions.Orchestrations
{
    public class AddModifyArticleOrchestration
    {
        [FunctionName(nameof(AddModifyArticleOrchestration))]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // Get input payload
            var githubApiUrl = context.GetInput<string>();

            // Get file contents and uri
            var fileNameContents = await context.CallActivityAsync<FileNameContents>(nameof(GetFileContentsActivity), githubApiUrl);
            var fileUri = await context.CallActivityAsync<Uri>(nameof(GetFileUriActivity), githubApiUrl);

            // Convert markdown to html
            var fileContentsHtml = await context.CallActivityAsync<string>(nameof(MarkdownToHtmlActivity), fileNameContents.FileContents);

            // Replace the markdown contents with html
            fileNameContents.FileContents = fileContentsHtml;

            // Upsert html blob to storage
            var htmlBlobUri = await context.CallActivityAsync<Uri>(nameof(UpsertBlobActivity), fileNameContents);

            // Yaml to Article
            var yamlToMarkdownActivityInput = new YamlToMarkdownActivityInput()
            {
                BlobPath = htmlBlobUri,
                FileContents = fileNameContents.FileContents,
                GithubPath = fileUri,
            };
            var article = await context.CallActivityAsync<Article>(nameof(YamlToMarkdownActivity), yamlToMarkdownActivityInput);


            var outputs = new List<string>()
            {
                $"Added/modified {githubApiUrl} - {fileContentsHtml}"
            };

            return outputs;
        }
    }
}
