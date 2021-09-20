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
        private readonly IStorageService _blobStorageService;

        public UpsertBlobActivity(IStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [FunctionName(nameof(UpsertBlobActivity))]
        public async Task<Uri> UpsertBlob([ActivityTrigger] ArticleContext articleContext)
        {
            // Chop off the .md to form a file name
            var fileName = articleContext.GithubContent.Name.Replace(".md", string.Empty);

            // Upsert blob from file contents
            return await _blobStorageService.UpsertBlob(fileName, articleContext.PlainHtmlContents);
        }
    }
}
