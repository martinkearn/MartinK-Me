using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace Workflow.Orchestrations
{
    public class MainOrchestration
    {
        [Function(nameof(MainOrchestration))]
        public async Task<string> RunMainOrchestration([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            string result = "";
            result += await context.CallActivityAsync<string>(("SayHello"), "Tokyo") + " ";
            result += await context.CallActivityAsync<string>(("SayHello"), "London") + " ";
            result += await context.CallActivityAsync<string>(("SayHello"), "Seattle");
            return result;
        }
    }

}