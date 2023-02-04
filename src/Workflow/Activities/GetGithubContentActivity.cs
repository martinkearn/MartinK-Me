using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

namespace Workflow.Activities
{
    public sealed class GetGithubContentActivity
    {
        private readonly IGithubService _githubService;

        public GetGithubContentActivity(IGithubService githubService)
        {
            _githubService = githubService;
        }

        [Function(nameof(GetGithubContentActivity))]
        public async Task<GithubContent> RunGetGithubContentActivity([ActivityTrigger] string gitHubApiFileUrl, FunctionContext executionContext)
        {
            var u = new Uri(gitHubApiFileUrl);
            return await _githubService.GetGithubContent(u);
        }
    }
}
