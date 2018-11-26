using MartinKMe.Interfaces;
using MartinKMe.Models.ArticlesViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MartinKMe.Controllers
{
    public class ArticlesController : Controller
    {

        private readonly IStore _store;

        public ArticlesController(IStore store)
        {
            _store = store;
        }

        public async Task<IActionResult> Index()
        {
            // all published articles
            var articles = await _store.GetContents();

            var vm = new IndexViewModel()
            {
                Articles = articles
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