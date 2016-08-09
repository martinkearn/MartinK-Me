using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MartinKRC2.Models;

namespace MartinKRC2.ViewModels.ResourcesViewModels
{
    public class EditViewModel
    {
        public Resource Resource { get; set; }

        public IEnumerable<SelectListItem> ResourceGroups { get; set; }

        public IEnumerable<SelectListItem> Talks { get; set; }
    }
}
