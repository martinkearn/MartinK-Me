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

        public async Task<Uri> UpsertBlob(string fileName, string fileContents)
        {
            // Get a reference to a blob
            var blobClient = _blobContainerClient.GetBlobClient(fileName);

            // Upload the file
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
            await blobClient.UploadAsync(ms, overwrite:true);

            return new Uri(blobClient.Uri.AbsoluteUri);
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
            // Build entitiy based on article. Table storage properties are case senitive and other systems using the same data expect the properties in camel case
            TableEntity entity = new TableEntity
            {
                PartitionKey = _partitionKey,
                RowKey = article.Key
            };
            
            entity[nameof(article.Key).ToLowerInvariant()] = article.Key;
            entity[nameof(article.Title).ToLowerInvariant()] = article.Title;
            entity[nameof(article.Author).ToLowerInvariant()] = article.Author;
            entity[nameof(article.Description).ToLowerInvariant()] = article.Description;
            entity[nameof(article.Image).ToLowerInvariant()] = article.Image;
            entity[nameof(article.Thumbnail).ToLowerInvariant()] = article.Thumbnail;
            entity[nameof(article.Published).ToLowerInvariant()] = DateTime.SpecifyKind(article.Published, DateTimeKind.Utc);
            entity[nameof(article.Categories).ToLowerInvariant()] = article.Categories;
            entity[nameof(article.Status).ToLowerInvariant()] = article.Status;
            entity[nameof(article.WebPath).ToLowerInvariant()] = article.WebPath;
            entity[nameof(article.HtmlBlobPath).ToLowerInvariant()] = article.HtmlBlobPath.ToString();

            // Upsert entity
            await _tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace);
        }

        public async Task DeleteArticle(string articleKey)
        {
            // Delete entity
            await _tableClient.DeleteEntityAsync(_partitionKey, articleKey);
        }
    }
}