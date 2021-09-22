using MartinKMe.Domain.Interfaces;
using MartinKMe.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace MartinKMe.Functions.Activities
{
    public sealed class DeleteBlobActivity
    {
        private readonly IStorageService _storageService;

        public DeleteBlobActivity(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [FunctionName(nameof(DeleteBlobActivity))]
        public async Task DeleteBlob([ActivityTrigger] ArticleContext articleContext)
        {
            await _storageService.DeleteBlob(articleContext.BlobFileName);
        }
    }
}
