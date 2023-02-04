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

        [Function(nameof(UpsertArticleActivity))]
        public async Task RunUpsertArticleActivity([ActivityTrigger] ArticleContext articleContext, FunctionContext executionContext)
        {
            await _storageService.UpsertArticle(articleContext.Article);
        }
    }
}
