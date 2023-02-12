using System;
using Azure;
using Azure.Data.Tables;
using Domain.Interfaces;

namespace Services.Models
{
    public class ShortcutEntity : ITableEntity, IShortcut
    {
        public string Key { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Group { get; set; }
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}

