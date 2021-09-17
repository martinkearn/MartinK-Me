using MartinKMe.Domain.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public sealed class GetFileUriActivity
    {
        private readonly IGithubService _githubService;

        public GetFileUriActivity(IGithubService githubService)
        {
            _githubService = githubService;
        }

        [FunctionName(nameof(GetFileUriActivity))]
        public async Task<Uri> GetFileUri([ActivityTrigger] string gitHubApiFileUrl)
        {
            return await _githubService.GetFileUri(gitHubApiFileUrl);
        }
    }
}
