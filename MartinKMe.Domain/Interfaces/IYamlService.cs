using MartinKMe.Domain.Models;
using System;

namespace MartinKMe.Domain.Interfaces
{
    /// <summary>
    /// Service for working with YAML.
    /// </summary>
    public interface IYamlService
    {
        /// <summary>
        /// Converts YAML to an Article object.
        /// </summary>
        /// <param name="plainFileContents">The plain file contents which contains the YAML.</param>
        /// <param name="blobUri">The Uri to set as the HtmlBlobPath property for the resulting Article.</param>
        /// <param name="articleKey">The key for the resulting Article. Used as Azure Storage entity row key.</param>
        /// <returns>An Article.</returns>
        public Article YamlToArticle(string plainFileContents, Uri blobUri, string articleKey);
    }
}
