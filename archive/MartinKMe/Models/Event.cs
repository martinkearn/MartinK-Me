using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models
{
    public class Event
    {
        public string Title { get; set; }

        public string ExternalUrl { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        public string City { get; set; }

        public string ImageUrl { get; set; }
    }
}
