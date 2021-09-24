using System;

namespace MartinKMe.Domain.Models
{
    /// <summary>
    /// Represents and Article's metadata. Derived from the YAML front matter in a file in Github.
    /// </summary>
    public class Article
    {
        /// <summary>
        /// The unique ID and Azure Storage Table entity key. Typically a base64 encoded version of the Github path, for example blogs/Test.md will result in a key of YmxvZ3MvdGVzdC5tZA==
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The title of the article
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The author of the article
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The description of the article
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// An absolute Url to an image to be used in the article itself
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// An absolute Url to a thumbnail image to be used in when listing the article amogst others
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// The date the article was published
        /// </summary>
        public DateTime Published { get; set; }

        /// <summary>
        /// A comma delimited list of tags or categories for the article
        /// </summary>
        public string Categories { get; set; }

        /// <summary>
        /// The status of the article, Typically either "draft" or "published"
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The absolute uri to the rleated html blob in Azure Storage. Does not require key or sas token.
        /// </summary>
        public Uri HtmlBlobPath { get; set; }
    }
}
