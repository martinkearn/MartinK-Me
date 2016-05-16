using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using MartinKMe.Models;

namespace MartinKMe.Controllers
{
    public class ResourceGroupsController : Controller
    {
        private ApplicationDbContext _context;

        public ResourceGroupsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: ResourceGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.ResourceGroup.ToListAsync());
        }

        // GET: ResourceGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            ResourceGroup resourceGroup = await _context.ResourceGroup.SingleAsync(m => m.Id == id);
            if (resourceGroup == null)
            {
                return HttpNotFound();
            }

            return View(resourceGroup);
        }

        // GET: ResourceGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ResourceGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResourceGroup resourceGroup)
        {
            if (ModelState.IsValid)
            {
                _context.ResourceGroup.Add(resourceGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(resourceGroup);
        }

        // GET: ResourceGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            ResourceGroup resourceGroup = await _context.ResourceGroup.SingleAsync(m => m.Id == id);
            if (resourceGroup == null)
            {
                return HttpNotFound();
            }
            return View(resourceGroup);
        }

        // POST: ResourceGroups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ResourceGroup resourceGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Update(resourceGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(resourceGroup);
        }

        // GET: ResourceGroups/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            ResourceGroup resourceGroup = await _context.ResourceGroup.SingleAsync(m => m.Id == id);
            if (resourceGroup == null)
            {
                return HttpNotFound();
            }

            return View(resourceGroup);
        }

        // POST: ResourceGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ResourceGroup resourceGroup = await _context.ResourceGroup.SingleAsync(m => m.Id == id);
            _context.ResourceGroup.Remove(resourceGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
