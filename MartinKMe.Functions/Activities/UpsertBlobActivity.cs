using MartinKMe.Domain.Interfaces;
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
        public async Task<Uri> UpsertBlob([ActivityTrigger] string blobContents)
        {
            // Get storage connection string. Use the same one as the function runtime
            var storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            // Decode the base64 blobContents string
            var decodedBlobContents = _utilityService.Base64Decode(blobContents);

            // Upsert blob from file contents
            var blobUri = await _blobStorageService.UpsertBlob("testing.html", decodedBlobContents, "contents", storageConnectionString);

            // Return
            return blobUri;
        }
    }
}
