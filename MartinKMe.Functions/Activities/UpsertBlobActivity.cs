using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public sealed class UpsertBlobActivity
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IUtilityService _utilityService;

        public UpsertBlobActivity(IBlobStorageService blobStorageService, IUtilityService utilityService)
        {
            _blobStorageService = blobStorageService;
            _utilityService = utilityService;
        }

        [FunctionName(nameof(UpsertBlobActivity))]
        public async Task<Uri> UpsertBlob([ActivityTrigger] FileNameContents input)
        {
            // Get storage connection string. Use the same one as the function runtime
            var storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            // Upsert blob from file contents
            return await _blobStorageService.UpsertBlob(input.FileName, input.FileContents, "contents", storageConnectionString);
        }
    }
}
