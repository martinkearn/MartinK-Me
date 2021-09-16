using MartinKMe.Domain.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public sealed class GetFileContentsActivity
    {
        private readonly IGithubService _githubService;

        public GetFileContentsActivity(IGithubService githubService)
        {
            _githubService = githubService;
        }

        [FunctionName(nameof(GetFileContentsActivity))]
        public async Task<string> GetFileContents([ActivityTrigger] string gitHubApiFileUrl)
        {
            // Get file contents
            var fileContents = await _githubService.GetFileContents(gitHubApiFileUrl);

            // Return
            return fileContents;
        }
    }
}
