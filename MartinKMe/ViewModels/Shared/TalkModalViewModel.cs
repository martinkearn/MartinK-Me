using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.ViewModels.Shared
{
    public class TalkModalViewModel
    {
        public string ElementId { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }

        public string Technologies { get; set; }

        public string Audience { get; set; }

        public string Time { get; set; }
    }
}
