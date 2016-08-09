using EvangelistSiteWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvangelistSiteWeb.Models.ArticlesViewModels
{
    public class IndexViewModel
    {
        public List<FeedItem> Articles { get; set; }
    }
}
