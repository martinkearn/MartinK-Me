using EvangelistSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvangelistSite.Models.ResourceGroupsViewModels
{
    public class EditViewModel
    {
        public ResourceGroup ResourceGroup { get; set; }

        public IEnumerable<Resource> Resources { get; set; }
    }
}
