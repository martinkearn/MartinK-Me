using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MartinKRC2.Models.ResourcesViewModels
{
    public class CreateViewModel
    {
        public Resource Resource { get; set; }

        public IEnumerable<SelectListItem> ResourceGroups { get; set; }
    }
}
