using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models.SearchViewModels
{
    public class IndexViewModel
    {
        public BingCustomSearchResponse SearchResponse { get; set; }

        public string SearchTerm { get; set; }
    }
}
