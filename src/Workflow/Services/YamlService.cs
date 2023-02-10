using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Workflow.Services
{
    /// <inheritdoc/>
    public class YamlService : IYamlService
    {
        public Article YamlToArticle(string plainFileContents, Article article)
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

            // Prepare categories to tags (url friendly)
            var tags = "";
            foreach (var cat in yamlHeader.Categories)
            {
                var tag = cat.Replace(' ', '-');
                tag = tag.ToLowerInvariant();
                tags += tag + ",";
            }
            tags = tags.TrimEnd(',');

            // Build Article
            if (article == null) article = new Article();
            article.Title = yamlHeader.Title;
            article.Author = yamlHeader.Author ?? string.Empty;
            article.Description = yamlHeader.Description ?? string.Empty;
            article.Image = yamlHeader.Image ?? string.Empty;
            article.Thumbnail = yamlHeader.Thumbnail ?? string.Empty;
            article.Published = yamlHeader.Published;
            article.Categories = tags;
            article.Status = yamlHeader.Status;

            // Return
            return article;
        }
    }
}
