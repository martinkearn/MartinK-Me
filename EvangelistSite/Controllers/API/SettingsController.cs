using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvangelistSite.Data;
using EvangelistSite.Models;

namespace EvangelistSite.Controllers.API
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

        // GET: api/Settings/5 or api/Settings/key
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSetting([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //see if it is an int, if so try id first. If it can't be parsed, the idInt variable will be 0
            int idInt = 0;
            int.TryParse(id, out idInt);

            Setting setting = (idInt == 0) ?
                await _context.Setting.SingleOrDefaultAsync(m => m.Key == id) :
                await _context.Setting.SingleOrDefaultAsync(m => m.Id == idInt);

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