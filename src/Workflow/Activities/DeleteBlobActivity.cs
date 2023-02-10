using Microsoft.Azure.Functions.Worker;

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
        public async Task RunDeleteBlobActivity([ActivityTrigger] string htmlBlobFileName, FunctionContext executionContext)
        {
            await _storageService.DeleteBlob(htmlBlobFileName);
        }
    }
}
