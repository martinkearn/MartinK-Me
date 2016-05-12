using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace MartinKMe.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Speaking()
        {
            return View();
        }

        public IActionResult Resources()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
