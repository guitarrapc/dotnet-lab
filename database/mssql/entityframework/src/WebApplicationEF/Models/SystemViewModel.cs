using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApplicationEF.Models
{
    public class SystemViewModel
    {
        private static readonly string hostName = System.Net.Dns.GetHostName();
        private readonly IPAddress hostIp = System.Net.Dns.GetHostAddresses(hostName)?.FirstOrDefault();
        public string HostName => hostName;
        public IPAddress HostIp => hostIp;
    }
}
