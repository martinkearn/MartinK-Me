using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKRC2.Models
{
    public class ResourceResourceGroup
    {
        public int ResourceGroupId { get; set; }
        public ResourceGroup ResourceGroup { get; set; }

        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}
