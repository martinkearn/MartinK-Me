using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models.ShortcutsViewModels
{
    public class IndexViewModel
    {
        public List<IGrouping<string,Shortcut>> Groups { get; set; }

        public string WallpaperUri { get; set; }
    }
}
