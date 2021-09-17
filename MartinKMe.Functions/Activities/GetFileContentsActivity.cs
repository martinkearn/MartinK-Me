using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
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
        public async Task<FileNameContents> GetFileContents([ActivityTrigger] string gitHubApiFileUrl)
        {
            return await _githubService.GetFileContents(gitHubApiFileUrl);
        }
    }
}
