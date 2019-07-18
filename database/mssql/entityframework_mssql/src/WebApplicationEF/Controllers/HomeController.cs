using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplicationEF.Data;
using WebApplicationEF.Models;

namespace WebApplicationEF.Controllers
{
    public class HomeController : Controller
    {
        private readonly BloggingContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(BloggingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var (isPool, poolCount) = DbConnectionMonitor.GetConnectionPoolCount(_context);
            var vm = new SystemViewModel()
            {
                ConnectionString = _configuration.GetValue<string>("ConnectionStrings:BloggingDatabase"),
                ConnectionPoolCount = poolCount,
                IsFromConnectionPool = isPool,
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
