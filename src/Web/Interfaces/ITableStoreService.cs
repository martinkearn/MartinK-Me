namespace Web.Interfaces
{
    /// <summary>
    /// Service for working with the Shortcuts table in Azure Storage.
    /// </summary>
    public interface ITableStoreService
    {
        /// <summary>
        /// Insert or update an entity.
        /// </summary>
        /// <param name="entity">Entity to upsert.</param>
        /// <returns>Task</returns>
        public Task Upsert(ITableEntity entity);

        /// <summary>
        /// Delete an entity based on entity key
        /// </summary>
        /// <param name="articleKey">The Azure Storage Table entity key to delete.</param>
        /// <returns>Task</returns>
        public Task Delete(string entityKey);

        /// <summary>
        /// Gets a list of entities
        /// </summary>
        /// <param name="filter">An OData filter string to be applied to the query. If ommited everything with the standard partition key will be used.</param>
        /// <param name="take">The number of articles to return. Sorted by most recent base don Published. Defaults to 5.</param>
        /// <returns>List of entities</returns>
        public List<ITableEntity> Query(string filter, int? take);
    }
}