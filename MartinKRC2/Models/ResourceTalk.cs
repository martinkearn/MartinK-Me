using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKRC2.Models
{
    public class ResourceTalk
    {
        public int TalkId { get; set; }
        public Talk Talk { get; set; }

        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}
