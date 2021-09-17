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

        public UpsertBlobActivity(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [FunctionName(nameof(UpsertBlobActivity))]
        public async Task<Uri> UpsertBlob([ActivityTrigger] FileNameContents input)
        {
            // Upsert blob from file contents
            return await _blobStorageService.UpsertBlob(input.FileName, input.FileContents);
        }
    }
}
