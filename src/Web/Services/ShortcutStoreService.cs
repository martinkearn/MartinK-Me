using Azure.Data.Tables;
using Domain.Models;
using Microsoft.Extensions.Options;
using Web.Interfaces;

namespace Web.Services
{
    public class ShortcutStoreService : ITableStoreService
    {
        private readonly StorageConfiguration _options;
        private readonly TableClient _tableClient;
        public ShortcutStoreService(IOptions<StorageConfiguration> storageConfigurationOptions)
        {
            _options = storageConfigurationOptions.Value;

            _tableClient = new TableClient(_options.ConnectionString, _options.ArticlesTable);
            _tableClient.CreateIfNotExists();
        }

        public Task Delete(string entityKey)
        {
            throw new NotImplementedException();
        }

        public List<ITableEntity> Query(string filter, int? take)
        {
            List<ShortcutEntity> shortcuts = new List<ShortcutEntity>();
        }

        public Task Upsert(ITableEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}