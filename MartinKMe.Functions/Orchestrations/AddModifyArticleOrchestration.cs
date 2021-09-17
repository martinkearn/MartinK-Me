﻿using MartinKMe.Functions.Activities;
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
            var githubApiUrl = context.GetInput<string>();

            // Get file contents
            var fileContentsMarkdown = await context.CallActivityAsync<string>(nameof(GetFileContentsActivity), githubApiUrl);

            // Convert markdown to html
            var fileContentsHtml = await context.CallActivityAsync<string>(nameof(MarkdownToHtmlActivity), fileContentsMarkdown);

            // Upsert html blob to storage
            var htmlBlobUri = await context.CallActivityAsync<string>(nameof(UpsertBlobActivity), fileContentsHtml);

            var outputs = new List<string>()
            {
                $"Added/modified {githubApiUrl} - {fileContentsHtml}"
            };

            return outputs;
        }
    }
}
