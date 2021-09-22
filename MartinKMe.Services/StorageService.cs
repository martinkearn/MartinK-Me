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
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_options.ConnectionString);

            // Get/create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_options.BlobContainer);
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
            entity["title"] = article.Title;
            entity["author"] = article.Author;
            entity["description"] = article.Description;
            entity["image"] = article.Image;
            entity["thumbnail"] = article.Thumbnail;
            entity["type"] = article.Type;
            entity["published"] = article.Published.ToString();
            entity["categories"] = article.Categories;
            entity["path"] = article.Path;
            entity["gitHubPath"] = article.GitHubPath;
            entity["status"] = article.Status;
            entity["htmlBlobPath"] = article.HtmlBlobPath.ToString();

            // Upsert entity
            await client.UpsertEntityAsync(entity, TableUpdateMode.Replace);
        }
    }
}
