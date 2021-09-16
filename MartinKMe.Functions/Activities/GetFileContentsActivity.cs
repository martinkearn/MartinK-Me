using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace MartinKMe.Functions.Activities
{
    internal class GetFileContentsActivity
    {
        [FunctionName(nameof(GetFileContentsActivity))]
        public static string GetFileContents([ActivityTrigger] string input)
        {
            return $"Hello from activity {input}!";
        }
    }
}
