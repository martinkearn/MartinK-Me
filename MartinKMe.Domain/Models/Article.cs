using System;

namespace MartinKMe.Domain.Models
{
    public class Article
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Thumbnail { get; set; }
        public string Type { get; set; }
        public DateTime Published { get; set; }
        public string Categories { get; set; }
        public string Path { get; set; }
        public Uri GitHubPath { get; set; }
        public string Status { get; set; }
        public Uri HtmlBlobPath { get; set; }
    }
}
