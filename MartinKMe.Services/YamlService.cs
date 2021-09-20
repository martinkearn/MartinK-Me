using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using System;
using System.IO;
using YamlDotNet.Serialization;

namespace MartinKMe.Services
{
    public class YamlService : IYamlService
    {
        private readonly IUtilityService _utilityService;

        public YamlService(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        public Article YamlToArticle(string plainFileContents, Uri blobUri, string githubPath)
        {
            var yamlString = plainFileContents.Substring(0, plainFileContents.LastIndexOf("---\n")); // Chop off the markdown, leaving just the YAML header as YamlDotNet only deals with YAML documents. Assumes there is a space after the end of the YAML header

            var yamlDeserializer = new DeserializerBuilder()
                .Build();

            var yamlHeader = yamlDeserializer.Deserialize<GithubFileYamlHeader>(yamlString);

            // Check we have required props
            if (string.IsNullOrEmpty(yamlHeader.Title))
            { 
                throw new ArgumentException("Title is a required field which is missing from the yaml header.", plainFileContents);
            }

            // Build dto
            var path = string.Join("-", yamlHeader.Title.Split(Path.GetInvalidFileNameChars()));
            path = path.Replace(" ", "-");
            path = path.ToLowerInvariant();
            var article = new Article()
            {
                Key = _utilityService.Base64Encode(githubPath.ToString()), // Base64 required because the path may contain a back slash
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
