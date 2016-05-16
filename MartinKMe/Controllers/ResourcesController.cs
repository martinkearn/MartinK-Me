using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MartinKMe.Models;

namespace MartinKMe.Controllers
{
    public class ResourcesController : Controller
    {
        private ApplicationDbContext _context;

        public ResourcesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Resources
        public async Task<IActionResult> Index()
        {
            return View(await _context.Resource.ToListAsync());
        }

        // GET: Resources/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Resources/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Resource resource)
        {
            if (ModelState.IsValid)
            {
                _context.Resource.Add(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(resource);
        }

        // GET: Resources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Resource resource = await _context.Resource.SingleAsync(m => m.Id == id);
            if (resource == null)
            {
                return HttpNotFound();
            }
            return View(resource);
        }

        // POST: Resources/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Resource resource)
        {
            if (ModelState.IsValid)
            {
                _context.Update(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(resource);
        }

        // GET: Resources/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Resource resource = await _context.Resource.SingleAsync(m => m.Id == id);
            if (resource == null)
            {
                return HttpNotFound();
            }

            return View(resource);
        }

        // POST: Resources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Resource resource = await _context.Resource.SingleAsync(m => m.Id == id);
            _context.Resource.Remove(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
