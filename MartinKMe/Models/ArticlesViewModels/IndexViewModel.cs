using MartinKMe.Models;
using Pioneer.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models.ArticlesViewModels
{
    public class IndexViewModel
    {
        public List<Content> Articles { get; set; }

        public int ItemsPerPage { get; set; }

        public int Pages { get; set; }

        public int ThisPage { get; set; }

        public int TotalItems { get; set; }

        public bool Drafts { get; set; }

    }
}
