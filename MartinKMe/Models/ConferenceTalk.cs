using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models
{
    public class ConferenceTalk
    {
        public int TalkId { get; set; }
        public Talk Talk { get; set; }

        public int ConferenceId { get; set; }
        public Conference Conference { get; set; }
    }
}
