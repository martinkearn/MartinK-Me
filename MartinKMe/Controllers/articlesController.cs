using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace MartinKMe.Controllers
{
    public class ArticlesController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("https://blogs.msdn.microsoft.com/martinkearn/");
        }


 
    }
}
