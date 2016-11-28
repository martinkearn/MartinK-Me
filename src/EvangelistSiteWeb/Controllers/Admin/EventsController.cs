using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EvangelistSiteWeb.Data;
using EvangelistSiteWeb.Models;
using EvangelistSiteWeb.Models.EventsViewModels;

namespace EvangelistSiteWeb.Controllers.Admin
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Event.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create()
        {
            var talks = new List<SelectListItem>();
            foreach (var item in await _context.Talk.ToListAsync())
            {
                talks.Add(new SelectListItem() { Text = item.Title, Value = item.Id.ToString() });
            }

            var vm = new CreateViewModel()
            {
                Ev = new Event(),
                Talks = talks
            };

            return View(vm);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            //TO DO @event is always null
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();

                //update Talks
                await CreateUpdateTalkMappings(@event.Id, Request.Form["TalkIds"].ToList());

                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.Include(o => o.EventTalks).SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            //get Event <> Talk mappings for this Event
            var selectedTalks = await _context.EventTalk.Where(o => o.EventId == id).ToListAsync();

            var talks = new List<SelectListItem>();
            foreach (var item in await _context.Talk.ToListAsync())
            {
                //figure out if this Talk is selected for this Event
                var isSelected = (selectedTalks.Where(o => o.TalkId == item.Id).Count() > 0);

                //construct and add Select List Item
                var selectListItem = new SelectListItem()
                {
                    Text = item.Title,
                    Value = item.Id.ToString(),
                    Selected = isSelected
                };
                talks.Add(selectListItem);
            }

            var vm = new EditViewModel()
            {
                Event = @event,
                Talks = talks
            };

            return View(vm);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //update Event
                    _context.Update(@event);
                    await _context.SaveChangesAsync();

                    //update Talks
                    await CreateUpdateTalkMappings(id, Request.Form["TalkIds"].ToList());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //delete Event <> Talk mappings
            await DeleteTalkMappings(id);

            //delete Event
            var @event = await _context.Event.SingleOrDefaultAsync(m => m.Id == id);
            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }

        private async Task<bool> CreateUpdateTalkMappings(int eventId, List<string> talkIds)
        {
            try
            {
                await DeleteTalkMappings(eventId);

                //add Event <> Talk mappings based on submitted form
                foreach (var talkId in talkIds)
                {
                    _context.EventTalk.Add(new EventTalk()
                    {
                        EventId = eventId,
                        TalkId = Convert.ToInt32(talkId)
                    });
                }
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> DeleteTalkMappings(int eventId)
        {
            try
            {
                _context.EventTalk.RemoveRange(_context.EventTalk.Where(o => o.EventId == eventId));
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
