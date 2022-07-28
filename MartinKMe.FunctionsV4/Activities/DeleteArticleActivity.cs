using MartinKMe.Domain.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace MartinKMe.FunctionsV4.Activities
{
    public sealed class DeleteArticleActivity
    {
        private readonly IStorageService _storageService;

        public DeleteArticleActivity(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [FunctionName(nameof(DeleteArticleActivity))]
        public async Task DeleteArticle([ActivityTrigger] string articleKey)
        {
            await _storageService.DeleteArticle(articleKey);
        }
    }
}
