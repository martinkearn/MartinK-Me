using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MartinKMe.Models;

namespace MartinKMe.Controllers
{
    public class TalksController : Controller
    {
        private ApplicationDbContext _context;

        public TalksController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Talks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Talk.ToListAsync());
        }

        // GET: Talks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Talk talk = await _context.Talk.SingleAsync(m => m.Id == id);
            if (talk == null)
            {
                return HttpNotFound();
            }

            return View(talk);
        }

        // GET: Talks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Talks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Talk talk)
        {
            if (ModelState.IsValid)
            {
                _context.Talk.Add(talk);
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
                return HttpNotFound();
            }

            Talk talk = await _context.Talk.SingleAsync(m => m.Id == id);
            if (talk == null)
            {
                return HttpNotFound();
            }
            return View(talk);
        }

        // POST: Talks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Talk talk)
        {
            if (ModelState.IsValid)
            {
                _context.Update(talk);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(talk);
        }

        // GET: Talks/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Talk talk = await _context.Talk.SingleAsync(m => m.Id == id);
            if (talk == null)
            {
                return HttpNotFound();
            }

            return View(talk);
        }

        // POST: Talks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Talk talk = await _context.Talk.SingleAsync(m => m.Id == id);
            _context.Talk.Remove(talk);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
