using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MartinKMe.Controllers
{
    public class RedirectController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(string tag)
        {
            return Redirect($"http://www.bing.com/search?q={tag}");
        }
    }
}
