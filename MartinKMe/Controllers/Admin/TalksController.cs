using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MartinKMe.Data;
using MartinKMe.Models;
using MartinKMe.Models.TalksViewModels;

namespace MartinKMe.Controllers
{
    public class TalksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TalksController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Talks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Talk.ToListAsync());
        }

        // GET: Talks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Talks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Audience,Description,Technologies,Time,Title,Url")] Talk talk)
        {
            if (ModelState.IsValid)
            {
                _context.Add(talk);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(talk);
        }

        // GET: Talks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var talk = await _context.Talk.SingleOrDefaultAsync(m => m.Id == id);
            if (talk == null)
            {
                return NotFound();
            }

            var vm = new EditViewModel()
            {
                Talk = talk
            };

            return View(vm);
        }

        // POST: Talks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Audience,Description,Technologies,Time,Title,Url")] Talk talk)
        {
            if (id != talk.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(talk);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TalkExists(talk.Id))
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
            return View(talk);
        }

        // GET: Talks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var talk = await _context.Talk.SingleOrDefaultAsync(m => m.Id == id);
            if (talk == null)
            {
                return NotFound();
            }

            return View(talk);
        }

        // POST: Talks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var talk = await _context.Talk.SingleOrDefaultAsync(m => m.Id == id);
            _context.Talk.Remove(talk);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TalkExists(int id)
        {
            return _context.Talk.Any(e => e.Id == id);
        }
    }
}
