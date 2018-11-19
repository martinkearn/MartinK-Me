using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models
{
    public class ContentDto
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
        public string htmlBase64 { get; set; }
        public string path { get; set; }
    }
}
