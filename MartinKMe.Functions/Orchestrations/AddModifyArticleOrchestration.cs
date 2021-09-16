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

            var outputs = new List<string>
            {
                await context.CallActivityAsync<string>(nameof(GetFileContentsActivity), input)
            };

            return outputs;
        }
    }
}
