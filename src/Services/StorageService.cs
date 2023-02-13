using System.Reflection;
using System.Text;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Options;
using Services.Models;
using System.Linq;

namespace Services
{
    /// <inheritdoc/>
    public class StorageService : IStorageService
    {
        private readonly StorageConfiguration _options;
        private readonly BlobContainerClient _articlesBlobContainerClient;
        private readonly TableClient _articlesTableClient;
        private const string _articlesPartitionKey = "article";
        private readonly TableClient _shortcutsTableClient;
        private const string _shortcutsPartitionKey = "shortcut";

        public StorageService(IOptions<StorageConfiguration> storageConfigurationOptions)
        { 
            _options = storageConfigurationOptions.Value;

            _articlesTableClient = new TableClient(_options.ConnectionString, _options.ArticlesTable);
            _articlesTableClient.CreateIfNotExists();

            _shortcutsTableClient = new TableClient(_options.ConnectionString, _options.ShortcutsTable);
            _shortcutsTableClient.CreateIfNotExists();

            _articlesBlobContainerClient = new BlobContainerClient(_options.ConnectionString, _options.ArticleBlobsContainer);
            _articlesBlobContainerClient.CreateIfNotExists();
        }

        public async Task<string> UpsertBlob(string fileName, string fileContents)
        {
            // Get a reference to a blob
            var blobClient = _articlesBlobContainerClient.GetBlobClient(fileName);

            // Upload the file
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
            await blobClient.UploadAsync(ms, overwrite:true);

            return blobClient.Uri.AbsoluteUri.ToString().ToLowerInvariant();
        }

        public async Task DeleteBlob(string fileName)
        {
            // Get a reference to a blob
            var blobClient = _articlesBlobContainerClient.GetBlobClient(fileName);

            // Delete blob
            await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
        }

        public async Task<string> GetBlobContent(string blobName)
        {
            var blobClient = _articlesBlobContainerClient.GetBlobClient(blobName);
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

        public async Task UpsertShortcut(Shortcut shortcut)
        {
            var entity = ConvertToTableEntity(shortcut, _shortcutsPartitionKey, shortcut.Key);
            await _shortcutsTableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
        }

        public async Task DeleteShortcut(string shortcutKey)
        {
            await _shortcutsTableClient.DeleteEntityAsync(_shortcutsPartitionKey, shortcutKey);
        }

        public List<Shortcut> QueryShortcuts(string filter, int? take)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = $"PartitionKey eq '{_shortcutsPartitionKey}'";
            }

            // Query shortcut entities
            var shortcutEntities = _shortcutsTableClient.Query<ShortcutEntity>(filter);

            // Cast ShortcutEntity to Shortcut
            var shortcuts = shortcutEntities.Select(x => new Shortcut() { Key = x.Key, Group = x.Group, Title = x.Title, Url = x.Url }).ToList();

            return shortcuts;
        }

        public async Task UpsertArticle(Article article)
        {
            var entity = ConvertToTableEntity(article, _articlesPartitionKey, article.Key);
            await _articlesTableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
        }

        public async Task DeleteArticle(string articleKey)
        {
            await _articlesTableClient.DeleteEntityAsync(_articlesPartitionKey, articleKey);
        }

        public List<Article> QueryArticles(string filter, bool includeDrafts, int? take)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = $"PartitionKey eq '{_articlesPartitionKey}'";
            }

            // Query article entities
            var articleEntities = _articlesTableClient.Query<ArticleEntity>(filter: filter);
            var articles = articleEntities.Select(x => new Article()
            {
                Key = x.Key,
                Title = x.Title,
                Author = x.Author,
                Description = x.Description,
                Image = x.Image,
                Thumbnail = x.Thumbnail,
                Published = x.Published,
                Categories = x.Categories,
                Status = x.Status,
                WebPath = x.WebPath,
                HtmlBlobPath = x.HtmlBlobPath,
                GitHubUrl = x.GitHubUrl,
                HtmlBlobFileName = x.HtmlBlobFileName
            }).ToList();

            //Sort by Published
            articles = articles.OrderByDescending(a => a.Published).ToList();

            // Remove drafts
            if (!includeDrafts)
            {
                articles = articles.Where(a => a.Status.ToLowerInvariant() == "published").ToList();
            }

            // Limit query if take is specified
            if (take != default)
            {
                var takeNotNullable = take ?? default(int);
                articles = articles.Take(takeNotNullable).ToList();
            }

            return articles;
        }

        public List<Article> GetArticlesByProperty(string property, string value)
        {
            if (string.IsNullOrEmpty(property))
            {
                return null;
            }
            var filter = $"{property} eq '{value}'";
            var articles = QueryArticles(filter, false, default);
            return articles;
        }

        public string Heartbeat()
        {
            return $"ArticleBlobsContainer is {_options.ArticleBlobsContainer}";
        }

        private TableEntity ConvertToTableEntity(object o, string partitionKey, string rowkey)
        {
            var entity = new TableEntity(partitionKey, rowkey);
            foreach (var prop in o.GetType().GetProperties())
            {
                var value = (prop.PropertyType.Name == nameof(DateTime)) ?
                    DateTime.SpecifyKind((DateTime)prop.GetValue(o), DateTimeKind.Utc) :
                    prop.GetValue(o);

                entity.Add(prop.Name, value);
            }
            return entity;
        }
    }
}
