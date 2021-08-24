using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using MartinKMe.Interfaces;
using MartinKMe.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace MartinKMe.Repositories
{
    public class Store : IStore
    {
        private readonly AppSecretSettings _appSecretSettings;
        private const string _linksContainer = "Links";
        private const string _linkPartitionkey = "Links";
        private const string _shortcutsContainer = "Shortcuts";
        private const string _shortcutPartitionkey = "Shortcuts";
        private const string _eventContainer = "Events";
        private const string _eventPartitionkey = "Events";
        private const string _talkContainer = "Talks";
        private const string _talkPartitionkey = "Talks";
        private const string _contentContainer = "Contents";
        private const string _articlePartitionkey = "article";
        private const string _wallpaperContainer = "wallpaper";

        public Store(IOptions<AppSecretSettings> appSecretSettings)
        {
            _appSecretSettings = appSecretSettings.Value;
        }

        public async Task<List<Link>> GetLinks()
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _linksContainer);

            TableContinuationToken token = null;

            var entities = new List<TableEntityAdapter<Link>>();

            TableQuery<TableEntityAdapter<Link>> query = new TableQuery<TableEntityAdapter<Link>>();

            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);
            
            var results = new List<Link>();
            foreach (var entity in entities)
            {
                var resultToBeAdded = entity.OriginalEntity;
                results.Add(resultToBeAdded);
            }

            return results;
        }

        public async Task<List<Shortcut>> GetShortcuts()
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _shortcutsContainer);

            TableContinuationToken token = null;

            var entities = new List<TableEntityAdapter<Shortcut>>();

            TableQuery<TableEntityAdapter<Shortcut>> query = new TableQuery<TableEntityAdapter<Shortcut>>();

            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            var results = new List<Shortcut>();
            foreach (var entity in entities)
            {
                var resultToBeAdded = entity.OriginalEntity;
                results.Add(resultToBeAdded);
            }

            return results;
        }

        public async Task<List<Event>> GetEvents(int take = 100)
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _eventContainer);

            TableContinuationToken token = null;

            var entities = new List<TableEntityAdapter<Event>>();

            TableQuery<TableEntityAdapter<Event>> query = new TableQuery<TableEntityAdapter<Event>>();

            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);
            
            var results = new List<Event>();
            foreach (var entity in entities)
            {
                var resultToBeAdded = entity.OriginalEntity;
                results.Add(resultToBeAdded);
            }

            var sortedList = results.OrderByDescending(o => o.Date)
                .Take(take)
                .ToList();

            return sortedList;
        }

        public async Task<List<Talk>> GetTalks()
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _talkContainer);

            TableContinuationToken token = null;

            var entities = new List<TableEntityAdapter<Talk>>();

            TableQuery<TableEntityAdapter<Talk>> query = new TableQuery<TableEntityAdapter<Talk>>();

            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            var results = new List<Talk>();
            foreach (var entity in entities)
            {
                var resultToBeAdded = entity.OriginalEntity;
                results.Add(resultToBeAdded);
            }

            var sortedList = results
                .ToList();

            return sortedList;
        }

        public async Task StoreLink(Link item)
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _eventContainer);

            TableBatchOperation batchOperation = new TableBatchOperation();

            var entity = new TableEntityAdapter<Link>(item, _linkPartitionkey, item.Tag.ToLower());
            batchOperation.InsertOrReplace(entity);

            await table.ExecuteBatchAsync(batchOperation);
        }

        public async Task StoreShortcut(Shortcut item)
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _shortcutsContainer);

            TableBatchOperation batchOperation = new TableBatchOperation();

            var entity = new TableEntityAdapter<Shortcut>(item, _shortcutPartitionkey, item.Title.Replace(" ", "-").ToLower());
            batchOperation.InsertOrReplace(entity);

            await table.ExecuteBatchAsync(batchOperation);
        }

        public async Task DeleteShortcut(string title)
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _shortcutsContainer);

            //get the Shortcut entity
            var shortcuts = await GetShortcuts();
            var shortcutToDelete = shortcuts.Where(o => o.Title.ToLowerInvariant() == title.ToLowerInvariant()).FirstOrDefault();
            var shortcutToDeleteEntity = new TableEntityAdapter<Shortcut>(shortcutToDelete, _shortcutPartitionkey, shortcutToDelete.Title.Replace(" ", "-").ToLower());
            shortcutToDeleteEntity.ETag = "*";

            // execute deletion operation
            TableBatchOperation batchOperation = new TableBatchOperation();
            batchOperation.Delete(shortcutToDeleteEntity);
            await table.ExecuteBatchAsync(batchOperation);
        }

        public async Task StoreEvent(Event item)
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _eventContainer);

            TableBatchOperation batchOperation = new TableBatchOperation();

            var rowKey = $"{FormatForUrl(item.Title)}-{FormatForUrl(item.Date.ToShortDateString())}";
            var entity = new TableEntityAdapter<Event>(item, _eventPartitionkey, rowKey);
            batchOperation.InsertOrReplace(entity);

            await table.ExecuteBatchAsync(batchOperation);
        }

        public async Task StoreTalk(Talk item)
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _talkContainer);

            TableBatchOperation batchOperation = new TableBatchOperation();

            var rowKey = $"{FormatForUrl(item.Title)}";
            var entity = new TableEntityAdapter<Talk>(item, _talkPartitionkey, rowKey);
            batchOperation.InsertOrReplace(entity);

            await table.ExecuteBatchAsync(batchOperation);
        }


        public async Task<List<Content>> GetContents()
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _contentContainer);

            TableContinuationToken token = null;

            var contentMetadataEntities = new List<TableEntityAdapter<ContentMetadata>>();

            //get published articles metadata
            var query = new TableQuery<TableEntityAdapter<ContentMetadata>>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _articlePartitionkey));
    
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                contentMetadataEntities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            // sort by date
            contentMetadataEntities.Sort((x, y) => y.OriginalEntity.published.CompareTo(x.OriginalEntity.published));

            // cast to results of type Content
            var results = new List<Content>();
            foreach (var contentMetadataEntitity in contentMetadataEntities)
            {
                var contentMetadata = contentMetadataEntitity.OriginalEntity;

                // Convert cats into collection
                var cats = contentMetadata.categories.Split(',').ToList();

                results.Add(new Content()
                {
                    Author = contentMetadata.author,
                    Categories = cats,
                    Description = contentMetadata.description,
                    Html = contentMetadata.htmlBlobPath, // To use to get the HTML when the individual item is used
                    Image = contentMetadata.image,
                    Key = contentMetadata.key,
                    Path = contentMetadata.path,
                    Published = Convert.ToDateTime(contentMetadata.published),
                    Thumbnail = contentMetadata.thumbnail,
                    Title = contentMetadata.title,
                    Type = contentMetadata.type,
                    Status = contentMetadata.status,
                    GitHubPath = "https://github.com/martinkearn/Content/blob/master/" + contentMetadata.gitHubPath
                });
            }

            return results;
        }

        public async Task<Content> GetContent(string id)
        {
            var contents = await GetContents();

            // Get file name and uri
            var thisItem = contents.Where(o => o.Path.ToLower() == id.ToLower()).FirstOrDefault();
            var thisItemUri = new Uri(thisItem.Html);
            var thisItemFileName = Path.GetFileName(thisItemUri.AbsoluteUri);

            // Download the blob into a string
            var blobContainer = new BlobContainerClient(_appSecretSettings.StorageConnectionString, _contentContainer.ToLower());
            var blobClient = blobContainer.GetBlobClient(thisItemFileName);
            var blobDownloadInfo = await blobClient.DownloadAsync();
            using (var streamReader = new StreamReader(blobDownloadInfo.Value.Content))
            {
                // Overwrite the value from GetContents which is a path to HTML with the actual HTML from the blob
                thisItem.Html = await streamReader.ReadToEndAsync();
            }

            return thisItem;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public async Task<List<string>> GetWallpaperUris()
        {
            var storageAccount = CloudStorageAccount.Parse(_appSecretSettings.StorageConnectionString);

            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(_wallpaperContainer);

            var wallpaperUris = new List<string>();
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    wallpaperUris.Add(item.Uri.ToString());
                }
            } while (blobContinuationToken != null); // Loop while the continuation token is not null.

            return wallpaperUris;
        }

        private static string FormatForUrl(string str)
        {
            var a = str.Replace(" ", "-");
            var b = a.Replace("/", "-");
            var c = Regex.Replace(b, "[^a-zA-Z0-9_.-]+", "", RegexOptions.Compiled);
            var d = c.ToLower();
            return d;
        }

        private async Task<CloudTable> GetCloudTable(string tableConnectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(tableConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(containerName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

    }
}
