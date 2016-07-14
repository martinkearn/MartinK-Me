using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MartinKRC2.Data;
using MartinKRC2.Models;
using MartinKRC2.ViewModels.ResourcesViewModels;
using Microsoft.AspNetCore.Http;

namespace MartinKRC2.Controllers
{
    public class ResourcesController : Controller
    {
        private readonly ApplicationDbContext _context;

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
        public async Task<IActionResult> Create()
        {
            var resourceGroups = new List<SelectListItem>();
            foreach (var item in await _context.ResourceGroup.ToListAsync())
            {
                resourceGroups.Add(new SelectListItem() { Text = item.Title, Value = item.Id.ToString() });
            }

            var vm = new CreateViewModel() {
                Resource = new Resource(),
                ResourceGroups = resourceGroups
            };

            return View(vm);
        }

        // POST: Resources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Resource resource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resource);
                await _context.SaveChangesAsync();

                //update Resource Groups
                await CreateUpdateResourceGroupMappings(resource.Id, Request.Form["ResourceGroupIds"].ToList());

                return RedirectToAction("Index");
            }
            return View(resource);
        }

        // GET: Resources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resource.Include(o => o.ResourceResourceGroups).SingleOrDefaultAsync(m => m.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            //get Resource <> Resource Group mappings for this Resource
            var selectedResourceGroups = await _context.ResourceResourceGroup.Where(o => o.ResourceId == resource.Id).ToListAsync();

            var resourceGroups = new List<SelectListItem>();
            foreach (var item in await _context.ResourceGroup.ToListAsync())
            {
                //figure out if this Resource Group is selected for this Resource
                var isSelected = (selectedResourceGroups.Where(o => o.ResourceGroupId == item.Id).Count() > 0);

                //construct and add Select List Item
                var selectListItem = new SelectListItem()
                {
                    Text = item.Title,
                    Value = item.Id.ToString(),
                    Selected = isSelected
                };
                resourceGroups.Add(selectListItem);
            }

            var vm = new EditViewModel()
            {
                Resource = resource,
                ResourceGroups = resourceGroups
            };

            return View(vm);
        }

        // POST: Resources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Resource resource)
        {
            if (id != resource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //update Resource
                    _context.Update(resource);
                    await _context.SaveChangesAsync();

                    //update Resource Groups
                    await CreateUpdateResourceGroupMappings(id, Request.Form["ResourceGroupIds"].ToList());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceExists(resource.Id))
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
            return View(resource);
        }

        // GET: Resources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resource.SingleOrDefaultAsync(m => m.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // POST: Resources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //delete Resource <> Resource Group mappings first
            await DeleteResourceGroupMappings(id);

            //delete Resource
            var resource = await _context.Resource.SingleOrDefaultAsync(m => m.Id == id);
            _context.Resource.Remove(resource);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool ResourceExists(int id)
        {
            return _context.Resource.Any(e => e.Id == id);
        }

        private async Task<bool> CreateUpdateResourceGroupMappings(int resourceId, List<string> resourceGroupIds)
        {
            try
            {
                await DeleteResourceGroupMappings(resourceId);

                //add Resource <> ResourceGroup mappings based on submitted form
                foreach (var resourceGroupId in resourceGroupIds)
                {
                    _context.ResourceResourceGroup.Add(new ResourceResourceGroup()
                    {
                        ResourceId = resourceId,
                        ResourceGroupId = Convert.ToInt32(resourceGroupId)
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

        private async Task<bool> DeleteResourceGroupMappings(int resourceId)
        {
            try
            {
                _context.ResourceResourceGroup.RemoveRange(_context.ResourceResourceGroup.Where(o => o.ResourceId == resourceId));
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
