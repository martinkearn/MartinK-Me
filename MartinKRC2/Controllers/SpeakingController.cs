using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MartinKRC2.Data;
using MartinKRC2.ViewModels.SpeakingViewModels;
using Microsoft.EntityFrameworkCore;
using MartinKRC2.Models;

namespace MartinKRC2.Controllers
{
    public class SpeakingController : Controller
    {
        private ApplicationDbContext _context;

        public SpeakingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var talks = await _context.Talk.ToListAsync();

            var vm = new IndexViewModel()
            {
                Talks = talks
            };

            return View(vm);
        }

        public async Task<IActionResult> Talk(string talk)
        {
            var thisTalk = await _context.Talk.Where(o => o.Url.ToLower() == talk).FirstOrDefaultAsync();

            //get Resource <> Talk mappings for this Talk
            var resourcesTalkMappings = await _context.ResourceTalk
                .Include(o => o.Resource)
                .Where(o => o.TalkId == thisTalk.Id)
                .ToListAsync();
            var resources = new List<Resource>();
            foreach (var rt in resourcesTalkMappings)
            {
                resources.Add(rt.Resource);
            }

            var vm = new TalkViewModel()
            {
                Talk = thisTalk,
                Resources = resources
            };

            return View(vm);
        }
    }
}