using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MartinKMe.Services
{
    public class YamlService : IYamlService
    {
        public Article YamlToArticle(string plainFileContents, Uri blobUri, string key)
        {
            var yamlString = plainFileContents.Substring(0, plainFileContents.LastIndexOf("---\n")); // Chop off the markdown, leaving just the YAML header as YamlDotNet only deals with YAML documents. Assumes there is a space after the end of the YAML header

            var yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
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
                Key = key,
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
                GitHubPath = key
            };

            // Return
            return article;
        }
    }
}
