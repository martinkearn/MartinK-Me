using MartinKMe.Interfaces;
using MartinKMe.Models;
using MartinKMe.Models.ShortcutsViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
            // wallpaper
            var wallpapers = await _store.GetWallpaperUris();
            var wallpaperIndex = (new Random().Next(1, (wallpapers.Count + 1 )))-1;
            var wallpaperUri = wallpapers[wallpaperIndex];

            // all shortcuts
            var allShortcuts = await _store.GetShortcuts();

            var orderedShortcuts = allShortcuts.OrderBy(o => o.Position).ToList();

            var groupedShortcuts = orderedShortcuts.GroupBy(o => o.Group).ToList();

            var vm = new IndexViewModel()
            {
                Groups = groupedShortcuts,
                WallpaperUri = wallpaperUri
            };

            return View(vm);
        }

        // POST: Shortcuts/Create
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var allShortcuts = await _store.GetShortcuts();
                var shortcutsInGroup = allShortcuts.Where(o => o.Group == collection["Group"]).OrderBy(o=>o.Position);
                var nextPosition = (shortcutsInGroup.Count() > 0) ?
                    shortcutsInGroup.Last().Position + 1 :
                    allShortcuts.OrderBy(o => o.Position).Last().Position +100;
                
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
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Shortcuts/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(IFormCollection collection)
        {
            try
            {
                var shortcutTitle = collection["Title"];

                await _store.DeleteShortcut(shortcutTitle);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

    }
}