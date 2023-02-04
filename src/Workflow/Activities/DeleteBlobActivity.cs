using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

namespace Workflow.Activities
{
    public sealed class DeleteBlobActivity
    {
        private readonly IStorageService _storageService;

        public DeleteBlobActivity(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [Function(nameof(DeleteBlobActivity))]
        public async Task RunDeleteBlobActivity([ActivityTrigger] ArticleContext articleContext, FunctionContext executionContext)
        {
            await _storageService.DeleteBlob(articleContext.BlobFileName);
        }
    }
}
