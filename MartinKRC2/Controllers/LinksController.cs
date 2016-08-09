using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MartinKRC2.Data;
using MartinKRC2.ViewModels.LinksViewModels;
using Microsoft.EntityFrameworkCore;
using MartinKRC2.Models;

namespace MartinKRC2.Controllers
{
    public class LinksController : Controller
    {
        private ApplicationDbContext _context;

        public LinksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var resourceGroups = await _context.ResourceGroup
                .Where(o => o.VisibleOnSite == true)
                .ToListAsync();

            var vm = new IndexViewModel()
            {
                ResourceGroups = resourceGroups
            };

            return View(vm);
        }

        public async Task<IActionResult> LinkGroup(string linkGroup)
        {
            var thisLinkGroup = await _context.ResourceGroup.Where(o => o.Url.ToLower() == linkGroup.ToLower()).FirstOrDefaultAsync();

            //get Resource <> Resource Group mappings for this link group
            var resourcesResourceGroupMappings = await _context.ResourceResourceGroup
                .Include(o => o.Resource)
                .Where(o => o.ResourceGroupId == thisLinkGroup.Id)
                .ToListAsync();
            var resources = new List<Resource>();
            foreach (var rrg in resourcesResourceGroupMappings)
            {
                resources.Add(rrg.Resource);
            }

            var vm = new LinkGroupViewModel()
            {
                ResourceGroup = thisLinkGroup,
                Resources = resources
            };

            return View(vm);
        }
    }
}