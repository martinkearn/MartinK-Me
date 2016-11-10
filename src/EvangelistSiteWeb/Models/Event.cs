using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvangelistSiteWeb.Models
{
    public class Event
    {
        public string Title { get; set; }
        public string Group { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string ExternalLink { get; set; }
        public string InternalLink { get; set; }
    }
}
