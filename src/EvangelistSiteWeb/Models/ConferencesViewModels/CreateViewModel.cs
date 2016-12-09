using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvangelistSiteWeb.Models.ConferencesViewModels
{
    public class CreateViewModel
    {
        public Conference Conference { get; set; }

        public IEnumerable<SelectListItem> Talks { get; set; }
    }
}
