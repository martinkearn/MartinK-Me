using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvangelistSiteWeb.Data;
using EvangelistSiteWeb.Models;

namespace EvangelistSiteWeb.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Settings")]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Settings
        [HttpGet]
        public IEnumerable<Setting> GetSetting()
        {
            return _context.Setting;
        }

        // GET: api/Settings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSetting([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Setting setting = await _context.Setting.SingleOrDefaultAsync(m => m.Id == id);

            if (setting == null)
            {
                return NotFound();
            }

            return Ok(setting);
        }

        private bool SettingExists(int id)
        {
            return _context.Setting.Any(e => e.Id == id);
        }
    }
}