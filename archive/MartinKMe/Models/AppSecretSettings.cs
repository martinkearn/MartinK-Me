using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinKMe.Models
{
    public class AppSecretSettings
    {
        public string StorageConnectionString { get; set; }

        public string DbConnectionString { get; set; }

        public string SearchSubscriptionKey { get; set; }

        public string SearchCustomConfigId { get; set; }
    }
}
