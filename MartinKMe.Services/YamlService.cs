using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MartinKMe.Services
{
    public class YamlService : IYamlService
    {
        private readonly IUtilityService _utilityService;

        public YamlService(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        public Article YamlToArticle(string yaml, Uri blobUri, string githubPath)
        {
            var yamlDeserializer = new DeserializerBuilder()
                .Build();

            var yamlHeader = yamlDeserializer.Deserialize<GithubFileYamlHeader>(yaml);

            // Check we have required props
            if (string.IsNullOrEmpty(yamlHeader.Title))
            { 
                throw new ArgumentException("Title is a required field which is missing from the yaml header.", yaml);
            }

            // Build dto
            var path = string.Join("-", yamlHeader.Title.Split(Path.GetInvalidFileNameChars()));
            path = path.Replace(" ", "-");
            path = path.ToLowerInvariant();
            var article = new Article()
            {
                Key = _utilityService.Base64Encode(githubPath), // Base64 required because the path may contain a back slash
                Title = yamlHeader.Title,
                Author = yamlHeader.Author ?? string.Empty,
                Description = yamlHeader.Description ?? string.Empty,
                Image = yamlHeader.Image ?? string.Empty,
                Thumbnail = yamlHeader.Thumbnail ?? string.Empty,
                Type = yamlHeader.Type.ToLowerInvariant() ?? string.Empty,
                Published = yamlHeader.Published,
                Categories = (string.Join(",", yamlHeader.Categories)) ?? string.Empty,
                HtmlBlobPath = blobUri,
                Path = path,
                Status = yamlHeader.Status,
                GitHubPath = githubPath
            };

            // Return
            return article;
        }
    }
}
