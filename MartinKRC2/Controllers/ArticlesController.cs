using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace MartinKRC2.Controllers
{
    public class ArticlesController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var feedUrl = "https://blogs.msdn.microsoft.com/martinkearn/feed/";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(feedUrl);
                var responseMessage = await client.GetAsync(feedUrl);
                var responseString = await responseMessage.Content.ReadAsStringAsync();
            }


            return View();
        }
    }
}