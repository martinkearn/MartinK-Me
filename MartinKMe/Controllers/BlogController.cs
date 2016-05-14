using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace MartinKMe.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


 
    }
}
