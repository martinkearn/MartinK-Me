using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Xml.Linq;
using MartinKMe.Models;
using MartinKMe.Models.ArticlesViewModels;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using MartinKMe.Interfaces;

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
            var articles = await _store.GetContents();

            var vm = new IndexViewModel()
            {
                Articles = articles
            };

            return View(vm);
        }
    }
}