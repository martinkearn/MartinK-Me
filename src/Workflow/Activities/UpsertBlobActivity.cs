using Microsoft.Azure.Functions.Worker;

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
        public async Task<string> RunUpsertBlobActivity([ActivityTrigger] ArticleContext articleContext, FunctionContext executionContext)
        {
            return await _storageService.UpsertBlob(articleContext.HtmlBlobFileName, articleContext.PlainHtmlContents);
        }
    }
}
