using BlazorAppEF.Data;
using BlazorAppEF.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppEF.Services
{
    public class SystemService
    {
        private readonly BloggingContext _context;
        private readonly IConfiguration _configuration;

        public SystemService(BloggingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public SystemModel GetSystemInfo()
        {
            var (isPool, poolCount) = DbConnectionMonitor.GetConnectionPoolCount(_context);
            return new SystemModel
            {
                ConnectionString = _configuration.GetValue<string>("ConnectionStrings:BloggingDatabase"),
                ConnectionPoolCount = poolCount,
                IsFromConnectionPool = isPool,
            };
        }
    }
}
