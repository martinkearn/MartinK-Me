using Microsoft.Azure.Functions.Worker;

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
        public async Task<GithubContent> RunGetGithubContentActivity([ActivityTrigger] string githubContentApiUri, FunctionContext executionContext)
        {
            return await _githubService.GetGithubContent(githubContentApiUri);
        }
    }
}
