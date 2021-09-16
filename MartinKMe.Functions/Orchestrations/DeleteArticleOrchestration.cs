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
            var input = context.GetInput<string>();

            var outputs = new List<string>()
            {
                $"Deleted {input}"
            };

            return outputs;
        }
    }
}
