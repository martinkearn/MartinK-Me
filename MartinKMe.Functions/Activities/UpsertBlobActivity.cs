using MartinKMe.Domain.Interfaces;
using MartinKMe.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public sealed class UpsertBlobActivity
    {
        private readonly IStorageService _storageService;

        public UpsertBlobActivity(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [FunctionName(nameof(UpsertBlobActivity))]
        public async Task<Uri> UpsertBlob([ActivityTrigger] ArticleContext articleContext)
        {
            return await _storageService.UpsertBlob(articleContext.GithubContent.Name, articleContext.PlainHtmlContents);
        }
    }
}
