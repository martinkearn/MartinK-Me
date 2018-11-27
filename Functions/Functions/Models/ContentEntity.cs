using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Models
{
    public class ContentEntity
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
        public string GitHubPath { get; set; }
        public string Status { get; set; }
        public string HtmlBlobPath { get; set; }
    }
}
