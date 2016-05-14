using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MartinKMe.ViewModels.Speaking;

namespace MartinKMe.Controllers
{
    public class SpeakingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Talk(string talk)
        {
            var title = "t";
            var description = "t";
            var audience = "t";
            var technologies = "t";
            var time = "t";
                
            var vm = new TalkViewModel()
            {
                Title = title,
                Description = description,
                Audience = audience,
                Technologies = technologies,
                Time = time
            };

            return View(vm);
        }


    }
}
