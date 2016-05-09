using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Display(Description = "Tag Label")]
        public string TagLabel { get; set; }

        public string Url { get; set; }
    }
}
