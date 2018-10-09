using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models
{
    public class Talk
    {

        public string Title { get; set; }

        public string Description { get; set; }

        public string Technologies { get; set; }

        public string Audience { get; set; }

        public string Time { get; set; }

        public string Url { get; set; }
    }
}
