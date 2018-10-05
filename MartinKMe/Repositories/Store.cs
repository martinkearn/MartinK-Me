using MartinKMe.Interfaces;
using MartinKMe.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MartinKMe.Repositories
{
    public class Store : IStore
    {
        private readonly AppSecretSettings _appSecretSettings;
        private const string _linksContainer = "Links";
        private const string _linkPartitionkey = "Links";

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

            // create list of objects from the storage entities
            var links = new List<Link>();
            foreach (var entity in entities)
            {
                Link toBeAdded = entity.OriginalEntity;
                links.Add(toBeAdded);
            }

            return links;
        }

        public async Task StoreLink(Link item)
        {
            var table = await GetCloudTable(_appSecretSettings.StorageConnectionString, _linksContainer);

            // Create the batch operation.
            TableBatchOperation batchOperation = new TableBatchOperation();

            // Create a TableEntityAdapter based on the item
            TableEntityAdapter<Link> entity = new TableEntityAdapter<Link>(item, _linkPartitionkey, GetRowKey(item.Tag));
            batchOperation.InsertOrReplace(entity);

            // Execute the batch operation.
            await table.ExecuteBatchAsync(batchOperation);
        }

        private string GetRowKey(string id) => id.ToLower();

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
