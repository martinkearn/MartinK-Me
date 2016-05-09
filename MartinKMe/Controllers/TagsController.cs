using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MartinKMe.Models;
using Microsoft.AspNet.Authorization;

namespace MartinKMe.Controllers
{
    public class TagsController : Controller
    {
        private ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tag.ToListAsync());
        }

        // GET: Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Tag tag = await _context.Tag.SingleAsync(m => m.Id == id);
            if (tag == null)
            {
                return HttpNotFound();
            }

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Tag.Add(tag);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tag);
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Tag tag = await _context.Tag.SingleAsync(m => m.Id == id);
            if (tag == null)
            {
                return HttpNotFound();
            }
            return View(tag);
        }

        // POST: Tags/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Update(tag);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tag);
        }

        // GET: Tags/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Tag tag = await _context.Tag.SingleAsync(m => m.Id == id);
            if (tag == null)
            {
                return HttpNotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Tag tag = await _context.Tag.SingleAsync(m => m.Id == id);
            _context.Tag.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
