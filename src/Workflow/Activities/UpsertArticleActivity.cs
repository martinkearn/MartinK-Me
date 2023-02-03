using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;

namespace Workflow.Activities
{
    public sealed class UpsertArticleActivity
    {
        private readonly IStorageService _storageService;

        public UpsertArticleActivity(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [FunctionName(nameof(UpsertArticleActivity))]
        public async Task RunUpsertArticleActivity([ActivityTrigger] Article article, FunctionContext executionContext)
        {
            await _storageService.UpsertArticle(article);
        }
    }
}
