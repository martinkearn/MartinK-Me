using Azure;
using Azure.Data.Tables;

namespace Web.Models
{
    public class ShortcutEntity: ITableEntity
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Group { get; set; }
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default;
        public ETag ETag { get; set; } = default!;
    }
}