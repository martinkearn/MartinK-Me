namespace Workflow.Models
{
    /// <summary>
    /// A model for collating data related to an Article as it passes through the pipeline.
    /// </summary>
    public class ArticleContext
    {
        /// <summary>
        /// The full Uri to get information about a given file from the Github content Api
        /// </summary>
        public Uri GithubContentApiUri { get; set; }

        /// <summary>
        /// Result of calling the Github Content api for the given article. Contains many usefull properties such as path, download uri, contents and name
        /// </summary>
        public GithubContent GithubContent { get; set; }

        /// <summary>
        /// The plain (decoded) content of the file.
        /// </summary>
        public string PlainContents { get; set; }

        /// <summary>
        /// The plain (decoded) content of the file converted to html.
        /// </summary>
        public string PlainHtmlContents { get; set; }

        /// <summary>
        /// The uri for the html blob in storage
        /// </summary>
        public Uri HtmlBlobStorageUri { get; set; }

        /// <summary>
        /// Name of the HTML blob in storage, for exmaple test.html
        /// </summary>
        public string BlobFileName { get; set; }

        /// <summary>
        /// Article entitiy which gets stored and used by other systems.
        /// </summary>
        public Article Article { get; set; }
    }
}