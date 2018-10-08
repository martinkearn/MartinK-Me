using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MartinKMe.Data;
using Microsoft.EntityFrameworkCore;
using MartinKMe.Interfaces;
using MartinKMe.Models;

namespace MartinKMe.Controllers
{
    public class RedirectController : Controller
    {
        private ApplicationDbContext _db;
        private readonly IStore _store;

        public RedirectController(ApplicationDbContext db, IStore store)
        {
            _db = db;
            _store = store;
        }

        public async Task<IActionResult> Index(string tagLabel)
        {
            var links = await _store.GetLinks();
            var matchingLink = links.FirstOrDefault(o => o.Tag.ToLower() == tagLabel.ToLower());

            if (matchingLink != null)
            {
                return Redirect(matchingLink.Url);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


    }
}