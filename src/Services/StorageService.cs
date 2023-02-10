using System.Text;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Options;
using Services.Models;

namespace Services
{
    /// <inheritdoc/>
    public class StorageService : IStorageService
    {
        private readonly StorageConfiguration _options;
        private readonly TableClient _tableClient;
        private readonly BlobContainerClient _blobContainerClient;
        private const string _partitionKey = "article";

        public StorageService(IOptions<StorageConfiguration> storageConfigurationOptions)
        { 
            _options = storageConfigurationOptions.Value;

            _tableClient = new TableClient(_options.ConnectionString, _options.ArticlesTable);
            _tableClient.CreateIfNotExists();

            _blobContainerClient = new BlobContainerClient(_options.ConnectionString, _options.ArticleBlobsContainer);
            _blobContainerClient.CreateIfNotExists();
        }

        public async Task<string> UpsertBlob(string fileName, string fileContents)
        {
            // Get a reference to a blob
            var blobClient = _blobContainerClient.GetBlobClient(fileName);

            // Upload the file
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
            await blobClient.UploadAsync(ms, overwrite:true);

            return blobClient.Uri.AbsoluteUri.ToString().ToLowerInvariant();
        }

        public async Task DeleteBlob(string fileName)
        {
            // Get a reference to a blob
            var blobClient = _blobContainerClient.GetBlobClient(fileName);

            // Delete blob
            await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
        }

        public async Task UpsertArticle(Article article)
        {
            // Create ArticleEntity based on Article
            var entity = new ArticleEntity();
            // Entity properties
            entity.RowKey = article.Key;
            entity.PartitionKey = _partitionKey;

            // Article properties
            entity.Key = article.Key;
            entity.Title = article.Title;
            entity.Author = article.Author;
            entity.Description = article.Description;
            entity.Image = article.Image;
            entity.Thumbnail = article.Thumbnail;
            entity.Published = DateTime.SpecifyKind(article.Published, DateTimeKind.Utc);
            entity.Categories = article.Categories;
            entity.Status = article.Status;
            entity.WebPath = article.WebPath;
            entity.HtmlBlobPath = article.HtmlBlobPath;
            entity.GitHubUrl = article.GitHubUrl;
            entity.HtmlBlobFileName = article.HtmlBlobFileName;

            // Upsert entity
            await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
        }

        public async Task DeleteArticle(string articleKey)
        {
            // Delete entity
            await _tableClient.DeleteEntityAsync(_partitionKey, articleKey);
        }

        public List<Article> QueryArticles(string filter, int? take)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = $"PartitionKey eq '{_partitionKey}'";
            }

            // Query article entities
            var articleEntities = _tableClient.Query<ArticleEntity>(filter: filter);
            articleEntities.OrderByDescending(a => a.Published);

            var articles = new List<Article>();
            foreach (var articleEntity in articleEntities)
            {
                var article = new Article()
                {
                    Key = articleEntity.Key,
                    Title = articleEntity.Title,
                    Author = articleEntity.Author,
                    Description = articleEntity.Description,
                    Image = articleEntity.Image,
                    Thumbnail = articleEntity.Thumbnail,
                    Published = articleEntity.Published,
                    Categories = articleEntity.Categories,
                    Status = articleEntity.Status,
                    WebPath = articleEntity.WebPath,
                    HtmlBlobPath = articleEntity.HtmlBlobPath,
                    GitHubUrl = articleEntity.GitHubUrl,
                    HtmlBlobFileName = articleEntity.HtmlBlobFileName
                };
                articles.Add(article);
            }

            // Limit query if take is specified
            if (take != default)
            {
                var takeNotNullable = take ?? default(int);
                articles = articles.Take(takeNotNullable).ToList();
            }

            // Remove drafts
            articles = articles.Where(a => a.Status.ToLowerInvariant() == "published").ToList();

            return articles;
        }

        public List<Article> GetArticlesByProperty(string property, string value)
        {
            if (string.IsNullOrEmpty(property))
            {
                return null;
            }
            var filter = $"{property} eq '{value}'";
            var articles = QueryArticles(filter, default);
            return articles;
        }

        public async Task<string> GetBlobContent(string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            var blobDownloadInfo = await blobClient.DownloadAsync();
            string contents;
            using (var streamReader = new StreamReader(blobDownloadInfo.Value.Content))
            {
                contents = await streamReader.ReadToEndAsync();
            }
            if (contents.StartsWith("<p>."))
            {
                var contentsWithoutOpeningP = contents.Substring(4);
                contents = $"<p>{contentsWithoutOpeningP}";
            }
            return contents;
        }

        public string Heartbeat()
        {
            return $"ArticleBlobsContainer is {_options.ArticleBlobsContainer}";
        }
    }
}
