using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EvangelistSiteWeb.Data;
using EvangelistSiteWeb.Models;
using EvangelistSiteWeb.Models.ResourceGroupsViewModels;

namespace EvangelistSiteWeb.Controllers
{
    public class ResourceGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResourceGroupsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: ResourceGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.ResourceGroup.ToListAsync());
        }

        // GET: ResourceGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ResourceGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CssClass,Description,ImageUrl,Title,VisibleOnSite,Url")] ResourceGroup resourceGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resourceGroup);
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
                return NotFound();
            }

            var resourceGroup = await _context.ResourceGroup.Include(o => o.ResourceResourceGroups).SingleOrDefaultAsync(m => m.Id == id);
            if (resourceGroup == null)
            {
                return NotFound();
            }

            //get Resource <> Resource Group mappings for this Resource Group
            var resourcesResourceGroupMappings = await _context.ResourceResourceGroup
                .Include(o => o.Resource)
                .Where(o => o.ResourceGroupId == id)
                .ToListAsync();
            var resources = new List<Resource>();
            foreach (var rrg in resourcesResourceGroupMappings)
            {
                resources.Add(rrg.Resource);
            }

            var vm = new EditViewModel()
            {
                ResourceGroup = resourceGroup,
                Resources = resources
            };

            return View(vm);
        }

        // POST: ResourceGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CssClass,Description,ImageUrl,Title,VisibleOnSite,Url")] ResourceGroup resourceGroup)
        {
            if (id != resourceGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resourceGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceGroupExists(resourceGroup.Id))
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
            return View(resourceGroup);
        }

        // GET: ResourceGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceGroup = await _context.ResourceGroup.SingleOrDefaultAsync(m => m.Id == id);
            if (resourceGroup == null)
            {
                return NotFound();
            }

            return View(resourceGroup);
        }

        // POST: ResourceGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resourceGroup = await _context.ResourceGroup.SingleOrDefaultAsync(m => m.Id == id);
            _context.ResourceGroup.Remove(resourceGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ResourceGroupExists(int id)
        {
            return _context.ResourceGroup.Any(e => e.Id == id);
        }
    }
}
