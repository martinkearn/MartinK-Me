using MartinKMe.Interfaces;
using MartinKMe.Models;
using MartinKMe.Models.ArticlesViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Controllers
{
    public class ArticlesController : Controller
    {
        private const int _itemsPerPage = 10;
        private readonly IStore _store;

        public ArticlesController(IStore store)
        {
            _store = store;
        }

        public async Task<IActionResult> Index(int page = 1, bool drafts = false)
        {
            // all published articles
            var allArticles = await _store.GetContents();

            //filter on status if drafts are false
            List<Content> articles = drafts ? allArticles : allArticles.Where(o => o.Status == "published").ToList();

            // get page
            var pageOfArticles = articles
                .Skip((page-1) * _itemsPerPage)
                .Take(_itemsPerPage)
                .ToList();

            // calculate number of pages
            var pagesCount = (double)articles.Count / (double)_itemsPerPage;
            int pageCountRounded = Convert.ToInt16(Math.Ceiling(pagesCount));

            var vm = new IndexViewModel()
            {
                Articles = pageOfArticles,
                ItemsPerPage = _itemsPerPage,
                Pages = pageCountRounded,
                ThisPage = page,
                TotalItems = articles.Count,
                Drafts = drafts
            };

            return View(vm);
        }

        public async Task<IActionResult> Article(string article)
        {
            var thisItem = await _store.GetContent(article);

            var vm = new ArticleViewModel()
            {
                Article = thisItem,
                Html = new HtmlString(thisItem.Html)
            };

            return View(vm);
        }
    }
}