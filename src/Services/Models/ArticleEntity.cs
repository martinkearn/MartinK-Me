using Azure;
using Azure.Data.Tables;
using Domain.Interfaces;
using Domain.Models;

namespace Services.Models
{
    /// <inheritdoc/>
	public class ArticleEntity : ITableEntity, IArticle
	{
        public ArticleEntity(Article article, string partitionKey)
		{
            // Entity properties
            RowKey = article.Key;
            PartitionKey = partitionKey;
            Timestamp = DateTimeOffset.UtcNow;

            // Article properties
            Key = article.Key;
            Title = article.Title;
            Author = article.Author;
            Description = article.Description;
            Image = article.Image;
            Thumbnail = article.Thumbnail;
            Published = DateTime.SpecifyKind(article.Published, DateTimeKind.Utc);
            Categories = article.Categories;
            Status = article.Status;
            WebPath = article.WebPath;
            HtmlBlobPath = article.HtmlBlobPath;
        }
        public string Key { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Image { get; set; } = default!;
        public string Thumbnail { get; set; } = default!;
        public DateTime Published { get; set; } = default!;
        public string Categories { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string WebPath { get; set; } = default!;
        public string HtmlBlobPath { get; set; } = default!;
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
    }
}

