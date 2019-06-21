using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplicationEF.Data;
using WebApplicationEF.Models;

namespace WebApplicationEF.Controllers
{
    public class HomeController : Controller
    {
        private readonly BloggingContext _context;

        public HomeController(BloggingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var (isPool, poolCount) = DbConnectionMonitor.GetConnectionPoolCount(_context);
            var vm = new SystemViewModel()
            {
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
