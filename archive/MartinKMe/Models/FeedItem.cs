using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models
{
    public class FeedItem
    {
        public string Link { get; set; } 
        public string Title { get; set; } 
        public string Description { get; set; } 
        public DateTime PublishDate { get; set; } 
    }
}
