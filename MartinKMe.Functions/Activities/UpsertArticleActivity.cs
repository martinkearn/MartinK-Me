using Azure;
using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public sealed class UpsertArticleActivity
    {
        private readonly IStorageService _storageService;

        public UpsertArticleActivity(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [FunctionName(nameof(UpsertArticleActivity))]
        public async Task UpsertBlob([ActivityTrigger] Article article)
        {
            await _storageService.UpsertArticle(article);
        }
    }
}
