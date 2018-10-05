using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MartinKMe.Data;
using Microsoft.EntityFrameworkCore;
using MartinKMe.Interfaces;
using MartinKMe.Models;

namespace MartinKMe.Controllers
{
    public class RedirectController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IStore _store;

        public RedirectController(ApplicationDbContext context, IStore store)
        {
            _context = context;
            _store = store;
        }

        public async Task<IActionResult> Index(string tagLabel)
        {
            var links = await _store.GetLinks();
            var matchingLink = links.FirstOrDefault(o => o.Tag.ToLower() == tagLabel.ToLower());


            var resources = await _context.Resource
                .Where(m => m.ShortUrl.ToLower() == tagLabel.ToLower())
                .ToListAsync();

            if (resources.Count > 0)
            {
                return Redirect(resources.FirstOrDefault().TargetUrl);
                //return Redirect($"http://www.bing.com/search?q={tag}");
            }
            else if (matchingLink != null)
            {
                return Redirect(matchingLink.Url);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}