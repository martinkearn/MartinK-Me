using MartinKMe.Interfaces;
using MartinKMe.Models;
using MartinKMe.Models.ArticlesViewModels;
using MartinKMe.Models.TagsViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Controllers
{
    public class TagsController : Controller
    {
        private const int _itemsPerPage = 10;
        private readonly IStore _store;

        public TagsController(IStore store)
        {
            _store = store;
        }

        public async Task<IActionResult> Tag(string tag)
        {
            // all published articles
            var allArticles = await _store.GetContents();

            //filter for just published articles
            List<Content> articles = allArticles.Where(o => o.Status == "published").ToList();

            // articles for tag
            var articlesInTag = new List<Content>();
            foreach (var article in articles)
            {
                foreach (var cat in article.Categories)
                {
                    if (cat.ToLowerInvariant() == tag.ToLowerInvariant()) articlesInTag.Add(article);
                }
            }

            var vm = new TagViewModel()
            {
                Articles = articlesInTag,
                Tag = tag
            };

            return View(vm);
        }
    }
}