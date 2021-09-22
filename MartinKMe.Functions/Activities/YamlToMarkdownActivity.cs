﻿using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using MartinKMe.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace MartinKMe.Functions.Activities
{
    public sealed class YamlToMarkdownActivity
    {
        private readonly IYamlService _yamlService;

        public YamlToMarkdownActivity(IYamlService yamlService)
        {
            _yamlService = yamlService;
        }

        [FunctionName(nameof(YamlToMarkdownActivity))]
        public Article YamlToMarkdown([ActivityTrigger] ArticleContext articleContext)
        {
            // Convert yaml to article
            return _yamlService.YamlToArticle(articleContext.PlainContents, articleContext.HtmlBlobStorageUri, articleContext.GithubContent.Path);
        }
    }
}