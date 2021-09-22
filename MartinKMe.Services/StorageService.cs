using Azure.Data.Tables;
using Azure.Storage.Blobs;
using MartinKMe.Domain.Interfaces;
using MartinKMe.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MartinKMe.Services
{
    public class StorageService : IStorageService
    {
        private readonly StorageConfiguration _options;

        public StorageService(IOptions<StorageConfiguration> storageConfigurationOptions)
        { 
            _options = storageConfigurationOptions.Value;
        }

        public async Task<Uri> UpsertBlob(string fileName, string fileContents)
        {
            // Get/create the container and return a container client object
            BlobContainerClient containerClient = new BlobContainerClient(_options.ConnectionString, _options.BlobContainer);
            await containerClient.CreateIfNotExistsAsync();

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Upload the file
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
            await blobClient.UploadAsync(ms, overwrite:true);

            return new Uri(blobClient.Uri.AbsoluteUri);
        }

        public async Task UpsertArticle(Article article)
        {
            // Create table if it does not exist
            TableClient client = new TableClient(_options.ConnectionString, _options.Table);
            await client.CreateIfNotExistsAsync();

            // Build entitiy based on article. Table storage properties are case senitive and other systems using the same data expect the properties in camel case
            TableEntity entity = new TableEntity
            {
                PartitionKey = "article",
                RowKey = article.Key
            };
            entity[nameof(article.Key).ToLowerInvariant()] = article.Key;
            entity[nameof(article.Title).ToLowerInvariant()] = article.Title;
            entity[nameof(article.Author).ToLowerInvariant()] = article.Author;
            entity[nameof(article.Description).ToLowerInvariant()] = article.Description;
            entity[nameof(article.Image).ToLowerInvariant()] = article.Image;
            entity[nameof(article.Thumbnail).ToLowerInvariant()] = article.Thumbnail;
            entity[nameof(article.Type).ToLowerInvariant()] = article.Type;
            entity[nameof(article.Published).ToLowerInvariant()] = new DateTime(article.Published.Ticks, DateTimeKind.Utc);
            entity[nameof(article.Categories).ToLowerInvariant()] = article.Categories;
            entity[nameof(article.Path).ToLowerInvariant()] = article.Path;
            entity[nameof(article.GitHubPath).ToLowerInvariant()] = article.GitHubPath;
            entity[nameof(article.Status).ToLowerInvariant()] = article.Status;
            entity[nameof(article.HtmlBlobPath).ToLowerInvariant()] = article.HtmlBlobPath.ToString();

            // Upsert entity
            await client.UpsertEntityAsync(entity, TableUpdateMode.Replace);
        }
    }
}
