using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvangelistSiteWeb.Models
{
    public class EventTalk
    {
        public int TalkId { get; set; }
        public Talk Talk { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
