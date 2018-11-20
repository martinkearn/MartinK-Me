using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models.ArticlesViewModels
{
    public class ArticleViewModel
    {
        public Content Article { get; set; }

        public HtmlString Html { get; set; }
    }
}
