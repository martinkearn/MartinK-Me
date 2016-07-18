using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MartinKRC2.Data;
using MartinKRC2.ViewModels.LinksViewModels;
using Microsoft.EntityFrameworkCore;

namespace MartinKRC2.Controllers
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
            var resourceGroups = await _context.ResourceGroup
                .Where(o => o.VisibleOnSite == true)
                .ToListAsync();

            var vm = new IndexViewModel()
            {
                ResourceGroups = resourceGroups
            };

            return View(vm);
        }
    }
}