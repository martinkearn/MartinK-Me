using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public sealed class UpsertArticleActivity
    {
        private readonly IStorageService _blobStorageService;

        public UpsertArticleActivity(IStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [FunctionName(nameof(UpsertArticleActivity))]
        public async Task UpsertBlob([ActivityTrigger] Article article)
        {

        }
    }
}
