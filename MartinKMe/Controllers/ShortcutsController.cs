using MartinKMe.Interfaces;
using MartinKMe.Models;
using MartinKMe.Models.ShortcutsViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MartinKMe.Controllers
{
    public class ShortcutsController : Controller
    {
        private readonly IStore _store;

        public ShortcutsController(IStore store)
        {
            _store = store;
        }

        public async Task<IActionResult> Index()
        {
            // all shortcuts
            var allShortcuts = await _store.GetShortcuts();

            var orderedShortcuts = allShortcuts.OrderBy(o => o.Position).ToList();

            var groupedShortcuts = orderedShortcuts.GroupBy(o => o.Group).ToList();

            var vm = new IndexViewModel()
            {
                Groups = groupedShortcuts
            };

            return View(vm);
        }

        // POST: Shortcuts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var allShortcuts = await _store.GetShortcuts();
                var shortcutsInGroup = allShortcuts.Where(o => o.Group == collection["Group"]).OrderBy(o=>o.Position);
                var nextPosition = shortcutsInGroup.Last().Position + 1;

                var shortcut = new Shortcut
                {
                    Title = collection["Title"],
                    Url = collection["Url"],
                    Group = collection["Group"],
                    Position = nextPosition
                };

                await _store.StoreShortcut(shortcut);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}