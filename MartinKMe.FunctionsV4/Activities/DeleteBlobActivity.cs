using MartinKMe.Domain.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace MartinKMe.FunctionsV4.Activities
{
    public sealed class DeleteBlobActivity
    {
        private readonly IStorageService _storageService;

        public DeleteBlobActivity(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [FunctionName(nameof(DeleteBlobActivity))]
        public async Task DeleteBlob([ActivityTrigger] string fileName)
        {
            await _storageService.DeleteBlob(fileName);
        }
    }
}
