using MartinKMe.Interfaces;
using MartinKMe.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string _eventContainer = "Events";
        private const string _eventPartitionkey = "Events";
        private const string _talkContainer = "Talks";
        private const string _talkPartitionkey = "Talks";
        private const string _contentContainer = "Contents";
        private const string _articlePartitionkey = "article";

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

            var entity = new TableEntityAdapter<Link>(item, _eventPartitionkey, item.Tag.ToLower());
            batchOperation.InsertOrReplace(entity);

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

            var entities = new List<TableEntityAdapter<ContentDto>>();

            TableQuery<TableEntityAdapter<ContentDto>> query = new TableQuery<TableEntityAdapter<ContentDto>>();

            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            // issue with case of properties in table so casting to my own object class
            var results = new List<Content>();
            foreach (var entity in entities)
            {
                var resultToBeAdded = entity.OriginalEntity;

                // Convert cats into collection
                var cats = resultToBeAdded.categories.Split(',').ToList();

                // Decode html
                var htmlBase64Bytes = Convert.FromBase64String(resultToBeAdded.htmlBase64);
                var html = Encoding.UTF8.GetString(htmlBase64Bytes);

                results.Add(new Content()
                {
                    Author = resultToBeAdded.author,
                    Categories = cats,
                    Description = resultToBeAdded.description,
                    Html = html,
                    Image = resultToBeAdded.image,
                    Key = resultToBeAdded.key,
                    Path = resultToBeAdded.path,
                    Published = Convert.ToDateTime(resultToBeAdded.published),
                    Thumbnail = resultToBeAdded.thumbnail,
                    Title = resultToBeAdded.title,
                    Type = resultToBeAdded.type
                });
            }

            // sort by date
            results.Sort((x, y) => y.Published.CompareTo(x.Published));

            return results;
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
            var blobClient = storageAccount.CreateCloudTableClient();
            var table = blobClient.GetTableReference(containerName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

    }

    class ContentDto
    {
        public string key { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string thumbnail { get; set; }
        public string type { get; set; }
        public string published { get; set; }
        public string categories { get; set; }
        public string htmlBase64 { get; set; }
        public string path { get; set; }
    }
}
