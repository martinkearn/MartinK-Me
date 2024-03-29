﻿using Microsoft.Azure.Functions.Worker;

namespace Workflow.Activities
{
    public sealed class DeleteArticleActivity
    {
        private readonly IStorageService _storageService;

        public DeleteArticleActivity(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [Function(nameof(DeleteArticleActivity))]
        public async Task RunDeleteArticleActivity([ActivityTrigger] string articleKey, FunctionContext executionContext)
        {
            await _storageService.DeleteArticle(articleKey);
        }
    }
}
