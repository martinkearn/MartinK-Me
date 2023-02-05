using Domain.Models;

namespace Domain.Interfaces
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
        /// <param name="article">The Article to append the YAML data onto.</param>
        /// <returns>An Article.</returns>
        public Article YamlToArticle(string plainFileContents, string blobUri, Article article);
    }
}
