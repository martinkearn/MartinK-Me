using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKRC2.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string FAIconClass { get; set; }

        public string Description { get; set; }

        public string ShortUrl { get; set; }

        public string TargetUrl { get; set; }

        public bool VisibleOnSite { get; set; }

        public virtual ICollection<ResourceResourceGroup> ResourceResourceGroups { get; set; }

        public virtual ICollection<ResourceTalk> ResourceTalks { get; set; }
    }
}
