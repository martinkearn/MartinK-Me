using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using System;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MartinKMe.Services
{
    /// <inheritdoc/>
    public class YamlService : IYamlService
    {
        public Article YamlToArticle(string plainFileContents, Uri blobUri, Article article)
        {
            // Check we have required props
            if (string.IsNullOrEmpty(article.Key))
            {
                throw new ArgumentException("Article.key property must already be set and is empty.", article.Key);
            }

            var yamlString = plainFileContents.Substring(0, plainFileContents.LastIndexOf("---\n")); // Chop off the markdown, leaving just the YAML header as YamlDotNet only deals with YAML documents. Assumes there is a space after the end of the YAML header

            var yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yamlHeader = yamlDeserializer.Deserialize<ArticleYamlHeader>(yamlString);

            // Ensure we have a Title
            if (string.IsNullOrEmpty(yamlHeader.Title))
            {
                throw new ArgumentException("Title is a required field which is missing from the yaml header.", plainFileContents);
            }

            // Build Path
            var pathSb = new StringBuilder(string.Join("-", yamlHeader.Title.Split(Path.GetInvalidFileNameChars())));
            pathSb.Replace(" ", "-");
            var path = pathSb.ToString().ToLowerInvariant();

            // Build Article
            if (article == null) article = new Article();
            article.Title = yamlHeader.Title;
            article.Author = yamlHeader.Author ?? string.Empty;
            article.Description = yamlHeader.Description ?? string.Empty;
            article.Image = yamlHeader.Image ?? string.Empty;
            article.Thumbnail = yamlHeader.Thumbnail ?? string.Empty;
            article.Published = yamlHeader.Published;
            article.Categories = (string.Join(",", yamlHeader.Categories)) ?? string.Empty;
            article.HtmlBlobPath = blobUri;
            article.Status = yamlHeader.Status;
            article.WebPath = path;

            // Return
            return article;
        }
    }
}
