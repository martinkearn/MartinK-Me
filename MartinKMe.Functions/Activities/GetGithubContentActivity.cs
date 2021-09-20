using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public sealed class GetGithubContentActivity
    {
        private readonly IGithubService _githubService;

        public GetGithubContentActivity(IGithubService githubService)
        {
            _githubService = githubService;
        }

        [FunctionName(nameof(GetGithubContentActivity))]
        public async Task<GithubContent> GetGithubContent([ActivityTrigger] string gitHubApiFileUrl)
        {
            var githubContent = await _githubService.GetGithubContent(gitHubApiFileUrl);
            return githubContent;
        }
    }
}
