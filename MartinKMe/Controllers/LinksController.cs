using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MartinKMe.Models;
using MartinKMe.ViewModels.Links;
using Microsoft.Data.Entity;

namespace MartinKMe.Controllers
{
    public class LinksController : Controller
    {
        private ApplicationDbContext _context;

        public LinksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var resourceGroups = await _context.ResourceGroup.Include(o => o.Resources).ToListAsync();

            var vm = new IndexViewModel()
            {
                ResourceGroups = resourceGroups
            };

            return View(vm);
        }



    }
}
