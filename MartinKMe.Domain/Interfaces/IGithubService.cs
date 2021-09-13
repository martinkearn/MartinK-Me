using System.Threading.Tasks;

namespace MartinKMe.Domain.Interfaces
{
    public interface IGithubService
    {
        /// <summary>
        /// Gets a file from a Github repo and returns the raw contents.
        /// </summary>
        /// <param name="author">The name of the Github user that the repo belongs to. For example "martinkearn".</param>
        /// <param name="repo">The name of the Github repo which contains the file. For exmaple "content".</param>
        /// <param name="filePath">The repo-relative path to the file. For exmaple "Blogs/Deploy dot net to Azure App Service with Pulumi.md"</param>
        /// <returns>The raw contents of the file as a string</returns>
        Task<string> GetFile(string author, string repo, string filePath);
    }
}
