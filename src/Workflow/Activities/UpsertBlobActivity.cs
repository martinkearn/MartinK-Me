using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

namespace Workflow.Activities
{
    public sealed class UpsertBlobActivity
    {
        private readonly IStorageService _storageService;

        public UpsertBlobActivity(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [Function(nameof(UpsertBlobActivity))]
        public async Task<Uri> RunUpsertBlobActivity([ActivityTrigger] ArticleContext articleContext, FunctionContext executionContext)
        {
            return await _storageService.UpsertBlob(articleContext.BlobFileName, articleContext.PlainHtmlContents);
        }
    }
}
