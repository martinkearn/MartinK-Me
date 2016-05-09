using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MartinKMe.Models;
using Microsoft.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MartinKMe.Controllers
{
    public class RedirectController : Controller
    {
        private ApplicationDbContext _context;

        public RedirectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(string tagLabel)
        {
            var tags = await _context.Tag
                .Where(m => m.TagLabel.ToLower() == tagLabel.ToLower())
                .ToListAsync();

            if (tags.Count > 0)
            {
                return Redirect(tags.FirstOrDefault().Url);
                //return Redirect($"http://www.bing.com/search?q={tag}");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
    }
}
