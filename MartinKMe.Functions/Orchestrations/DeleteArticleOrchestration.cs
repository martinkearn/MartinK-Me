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
            var outputs = new List<string>()
            {
                "Testing from DeleteArticleOrchestration"
            };

            return outputs;
        }
    }
}
