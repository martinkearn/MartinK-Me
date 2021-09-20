﻿using Azure.Data.Tables;
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

            return blobClient.Uri;
        }

        public async Task UpsertArticle(Article article)
        {
            // Create table if it does not exist
            TableClient client = new TableClient(_options.ConnectionString, _options.Table);
            await client.CreateIfNotExistsAsync();

            // Build entitiy based on article
            TableEntity entity = new TableEntity
            {
                PartitionKey = "article",
                RowKey = article.Key
            };
            entity[nameof(Article.Title)] = article.Title;
            entity[nameof(Article.Author)] = article.Author;
            entity[nameof(Article.Description)] = article.Description;
            entity[nameof(Article.Image)] = article.Image;
            entity[nameof(Article.Thumbnail)] = article.Thumbnail;
            entity[nameof(Article.Type)] = article.Type;
            entity[nameof(Article.Published)] = article.Published.ToString();
            entity[nameof(Article.Categories)] = article.Categories;
            entity[nameof(Article.Path)] = article.Path;
            entity[nameof(Article.GitHubPath)] = article.GitHubPath;
            entity[nameof(Article.Status)] = article.Status;
            entity[nameof(Article.HtmlBlobPath)] = article.HtmlBlobPath.ToString();

            // Upsert entity
            await client.UpsertEntityAsync(entity, TableUpdateMode.Replace);
        }
    }
}
