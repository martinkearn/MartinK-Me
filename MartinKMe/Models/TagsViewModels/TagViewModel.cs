using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models.TagsViewModels
{
    public class TagViewModel
    {
        public string Tag { get; set; }
        public List<Content> Articles { get; set; }

    }
}
