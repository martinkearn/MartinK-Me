using MartinKMe.Domain.Models;
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
        /// <returns>File name and the raw contents of the file as a string</returns>
        Task<FileNameContents> GetFileContents(string fileApiUrl);

        /// <summary>
        /// Gets the uri to view the file on Github
        /// </summary>
        /// <param name="fileApiUrl">The Github api url for the file </param>
        /// <returns>Uri for the file on Github</returns>
        Task<Uri> GetFileUri(string fileApiUrl);
    }
}
