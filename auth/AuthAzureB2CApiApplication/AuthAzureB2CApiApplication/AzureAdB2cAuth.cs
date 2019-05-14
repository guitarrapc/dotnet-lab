using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAzureB2CApiApplication
{
    public class AzureAdB2cAuth
    {
        public string Instance { get; set; }
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public string CallbackUrl { get; set; }
    }
}
