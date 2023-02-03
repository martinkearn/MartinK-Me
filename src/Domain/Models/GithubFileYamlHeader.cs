﻿namespace Domain.Models
{
    public class GithubFileYamlHeader
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Thumbnail { get; set; }
        public string Type { get; set; }
        public DateTime Published { get; set; }
        public List<string> Categories { get; set; }
        public string Status { get; set; }
    }
}
