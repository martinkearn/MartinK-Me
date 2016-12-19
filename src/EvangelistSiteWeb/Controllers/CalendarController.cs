using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EvangelistSiteWeb.Models;
using Microsoft.Extensions.Options;
using EvangelistSiteWeb.Data;
using Microsoft.EntityFrameworkCore;
using EvangelistSiteWeb.Models.CalendarViewModels;

namespace EvangelistSiteWeb.Controllers
{
    public class CalendarController : Controller
    {
        private ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var conferences = await _context.Conference
                .Where(o => o.Date > DateTime.Now)
                .Include(o => o.ConferenceTalks)
                .ThenInclude(o => o.Talk)
                .OrderBy(o => o.Date)
                .ToListAsync();

            var vm = new IndexViewModel()
            {
                Conferences = conferences
            };

            return View(vm);
        }
    }
}