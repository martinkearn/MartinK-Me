using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MartinKMe.Interfaces;
using MartinKMe.Models;
using MartinKMe.Models.EventsViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MartinKMe.Controllers
{
    public class EventsController : Controller
    {
        private readonly IStore _store;

        public EventsController(IStore store)
        {
            _store = store;
        }

        public async Task<IActionResult> Index()
        {
            List<Event> events = await _store.GetEvents(10);
           
            var vm = new IndexViewModel()
            {
                Events = events
            };

            return View(vm);
        }
    }
}