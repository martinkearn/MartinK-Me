using MartinKMe.Interfaces;
using MartinKMe.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
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
}
