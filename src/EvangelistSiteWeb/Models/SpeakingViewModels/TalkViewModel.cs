using EvangelistSiteWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvangelistSiteWeb.Models.SpeakingViewModels
{
    public class TalkViewModel
    {
        public Talk Talk { get; set; }

        public IEnumerable<Resource> Resources { get; set; }
    }
}
