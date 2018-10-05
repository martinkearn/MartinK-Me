using MartinKMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models.TalksViewModels
{
    public class EditViewModel
    {
        public Talk Talk { get; set; }

        public IEnumerable<Resource> Resources { get; set; }
    }
}
