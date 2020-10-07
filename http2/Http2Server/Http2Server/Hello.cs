using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Http2Server
{
    public class Hello
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Protocol { get; set; }
        public string HostName { get; set; }
    }
}
