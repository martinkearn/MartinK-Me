using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models
{
    public class ContentMetadata
    {
        public string key { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string thumbnail { get; set; }
        public string type { get; set; }
        public string published { get; set; }
        public string categories { get; set; }
        public string htmlBlobPath { get; set; }
        public string path { get; set; }
        public string status { get; set; }
        public string gitHubPath { get; set; }
    }
}
