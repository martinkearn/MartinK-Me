using System;
using System.Threading.Tasks;

namespace MartinKMe.Domain.Interfaces
{
    public interface IGithubService
    {
        /// <summary>
        /// Gets a file from a Github repo and returns the raw contents.
        /// </summary>
        /// <param name="fileApiUrl">The Github api url for the file to return.</param>
        /// <returns>The raw contents of the file as a string</returns>
        Task<string> GetFileContents(string fileApiUrl);
    }
}
