using Domain.Models;

namespace Domain.Interfaces
{
    /// <summary>
    /// Service for working with Azure Storage.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Insert or update a blob with new contents.
        /// </summary>
        /// <param name="fileName">The name of the blob i.e. test.md</param>
        /// <param name="fileContents">The new contents of the blob</param>
        /// <returns>Uri to the newly created or updated blob.</returns>
        public Task<string> UpsertBlob(string fileName, string fileContents);

        /// <summary>
        /// Delete a blob
        /// </summary>
        /// <param name="fileName">File name of the blob to delete i.e. Test.md</param>
        /// <returns>Task</returns>
        public Task DeleteBlob(string fileName);

        /// <summary>
        /// Insert or update an Article entity.
        /// </summary>
        /// <param name="article">Article to upsert.</param>
        /// <returns>Task</returns>
        public Task UpsertArticle(Article article);

        /// <summary>
        /// Delete an article based on entity key
        /// </summary>
        /// <param name="articleKey">The Azure Storage Table entity key to delete.</param>
        /// <returns>Task.</returns>
        public Task DeleteArticle(string articleKey);

        /// <summary>
        /// Gets a list of Article
        /// </summary>
        /// <param name="filter">An OData filter string to be applied to the query. If ommited everything with the standard partition key will be used.</param>
        /// <returns>List of articles</returns>
        public List<Article> QueryArticles(string filter);

        /// <summary>
        /// Function to use to test avaliability of service
        /// </summary>
        /// <returns></returns>
        public string Heartbeat();
    }
}
