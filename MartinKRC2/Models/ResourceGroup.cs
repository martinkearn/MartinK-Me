using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKRC2.Models
{
    public class ResourceGroup
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string CssClass { get; set; }

        public List<Resource> Resources { get; set; }

        public bool VisibleOnSite { get; set; }
    }
}
