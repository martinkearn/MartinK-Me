using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MartinKMe.Models.SpeakingViewModels;
using Microsoft.EntityFrameworkCore;
using MartinKMe.Models;
using MartinKMe.Interfaces;

namespace MartinKMe.Controllers
{
    public class SpeakingController : Controller
    {
        private readonly IStore _store;

        public SpeakingController(IStore store)
        {
            _store = store;
        }

        public async Task<IActionResult> Index()
        {
            var talks = await _store.GetTalks();

            var vm = new IndexViewModel()
            {
                Talks = talks
            };

            return View(vm);
        }

        public async Task<IActionResult> Talk(string talk)
        {
            var talks = await _store.GetTalks();

            var thisTalk = talks.Where(o => o.Url.ToLower() == talk.ToLower()).FirstOrDefault();

            var vm = new TalkViewModel()
            {
                Talk = thisTalk
            };

            return View(vm);
        }
    }
}