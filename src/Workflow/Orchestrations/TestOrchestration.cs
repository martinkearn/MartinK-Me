using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace Workflow.Orchestrations
{
    public class TestOrchestration
    {
        [Function(nameof(TestOrchestration))]
        public async Task<List<string>> RunTestOrchestration([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            List<string> input = context.GetInput<List<string>>();
            input.Add("bar");
            return input;
        }
    }
}