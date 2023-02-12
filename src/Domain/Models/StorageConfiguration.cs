namespace Domain.Models
{
    /// <summary>
    /// Used to strongly type the "StorageConfiguration" appsettings section
    /// </summary>
    public class StorageConfiguration
    {
        /// <summary>
        /// The name of the blob container in Azure Storage where HTML blobs for Articles are stored.
        /// </summary>
        public string ArticleBlobsContainer { get; set; }

        /// <summary>
        /// The connection string for Azure Storage.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The name of a table in Azure Storage where Article entity are stored.
        /// </summary>
        public string ArticlesTable { get; set; }

        /// <summary>
        /// The name of a table in Azure Storage where Shortcut entities are stored.
        /// </summary>
        public string ShortcutsTable { get; set; }        
    }
}
