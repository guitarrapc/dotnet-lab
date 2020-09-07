using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BlazorAppEF.Models
{
    public class SystemModel
    {
        private static readonly string hostName = System.Net.Dns.GetHostName();
        private readonly IPAddress _hostIp = System.Net.Dns.GetHostAddresses(hostName)?.FirstOrDefault();

        public string HostName => hostName;
        public IPAddress HostIp => _hostIp;
        public string ConnectionString { get; set; }
        public int ConnectionPoolCount { get; set; }
        public bool IsFromConnectionPool { get; set; }
    }
}
