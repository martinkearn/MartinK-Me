using Domain.Models;

namespace Domain.Interfaces
{
    /// <summary>
    /// Service for interacting with Github.
    /// </summary>
    public interface IGithubService
    {
        /// <summary>
        /// Gets a content for a given file from the Github Content api
        /// </summary>
        /// <param name="fileApiUrl">The Github api url for the file to return.</param>
        /// <returns>Github content for the file.</returns>
        Task<GithubContent> GetGithubContent(Uri fileApiUrl);
    }
}
