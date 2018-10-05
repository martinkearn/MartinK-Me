using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartinKMe.Data;
using MartinKMe.Models;

namespace MartinKMe.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Resources")]
    public class ResourcesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Resources
        [HttpGet]
        public IEnumerable<Resource> GetResource(string AlwaysOn)
        {
            bool isAlwaysOn;
            if (Boolean.TryParse(AlwaysOn, out isAlwaysOn))
                return _context.Resource.Where(o => o.AlwaysOn == isAlwaysOn);
            else
            {
                return _context.Resource;
            }
        }

        // GET: api/Resources/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResource([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Resource resource = await _context.Resource
                .Include(o => o.ResourceResourceGroups)
                .Include(o => o.ResourceTalks)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (resource == null)
            {
                return NotFound();
            }

            return Ok(resource);
        }

        private bool ResourceExists(int id)
        {
            return _context.Resource.Any(e => e.Id == id);
        }
    }
}