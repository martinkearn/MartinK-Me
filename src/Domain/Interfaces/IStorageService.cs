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
        public Task<Uri> UpsertBlob(string fileName, string fileContents);

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
    }
}
