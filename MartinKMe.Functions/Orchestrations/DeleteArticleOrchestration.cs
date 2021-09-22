﻿using MartinKMe.Functions.Activities;
using MartinKMe.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Orchestrations
{
    public class DeleteArticleOrchestration
    {
        [FunctionName(nameof(DeleteArticleOrchestration))]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // Get input payload
            var articleContext = context.GetInput<ArticleContext>();

            // Delete blob
            await context.CallActivityAsync<string>(nameof(DeleteBlobActivity), articleContext);

            // Delete article

            var outputs = new List<string>()
            {
                $"Deleted {articleContext.GithubContentApiUri}"
            };

            return outputs;
        }
    }
}
