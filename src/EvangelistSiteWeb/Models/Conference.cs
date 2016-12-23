using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EvangelistSiteWeb.Models
{
    public class Conference
    {
        [Key]
        public int Id { get; set; }
        public string ConferenceTitle { get; set; }
        public string ExternalUrl { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        public string City { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<ConferenceTalk> ConferenceTalks { get; set; }
    }
}
