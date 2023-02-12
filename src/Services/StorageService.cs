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
            // Create entity based on shortcut
            var te = new TableEntity(_shortcutsPartitionKey, Guid.NewGuid().ToString());
            foreach (var prop in shortcut.GetType().GetProperties())
            {
                te.Add(prop.Name, prop.GetValue(shortcut));
            }

            // Upsert entity
            await _shortcutsTableClient.UpsertEntityAsync(te, TableUpdateMode.Replace);
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
            var shortcuts = shortcutEntities.Select(x => new Shortcut() { Group = x.Group, Title = x.Title, Url = x.Url }).ToList();

            return shortcuts;
        }

        public async Task UpsertArticle(Article article)
        {
            //// Create ArticleEntity based on Article
            //var entity = new ArticleEntity();
            //entity.RowKey = article.Key;
            //entity.PartitionKey = _articlesPartitionKey;

            //// Article properties
            //entity.Key = article.Key;
            //entity.Title = article.Title;
            //entity.Author = article.Author;
            //entity.Description = article.Description;
            //entity.Image = article.Image;
            //entity.Thumbnail = article.Thumbnail;
            //entity.Published = DateTime.SpecifyKind(article.Published, DateTimeKind.Utc);
            //entity.Categories = article.Categories;
            //entity.Status = article.Status;
            //entity.WebPath = article.WebPath;
            //entity.HtmlBlobPath = article.HtmlBlobPath;
            //entity.GitHubUrl = article.GitHubUrl;
            //entity.HtmlBlobFileName = article.HtmlBlobFileName;

            var te = new TableEntity(_articlesPartitionKey, article.Key);
            foreach (var prop in article.GetType().GetProperties())
            {
                var value = (prop.PropertyType.Name == nameof(DateTime)) ?
                    DateTime.SpecifyKind((DateTime)prop.GetValue(article), DateTimeKind.Utc) :
                    prop.GetValue(article);

                te.Add(prop.Name, value);
            }

            // Upsert entity
            //await _articlesTableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
            await _articlesTableClient.UpsertEntityAsync(te, TableUpdateMode.Replace);
        }

        public async Task DeleteArticle(string articleKey)
        {
            // Delete entity
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

            // Limit query if take is specified
            if (take != default)
            {
                var takeNotNullable = take ?? default(int);
                articles = articles.Take(takeNotNullable).ToList();
            }

            // Remove drafts
            if (!includeDrafts)
            {
                articles = articles.Where(a => a.Status.ToLowerInvariant() == "published").ToList();
            }

            //Sort by Published
            articles = articles.OrderByDescending(a => a.Published).ToList();

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
    }
}
