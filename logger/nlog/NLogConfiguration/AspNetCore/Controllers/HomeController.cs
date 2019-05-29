using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.Models;
using Microsoft.Extensions.Logging;
using AspNetCore.Domains;

namespace AspNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFooService domain;
        private readonly ILogger<HomeController> logger;

        public HomeController(IFooService domain, ILogger<HomeController> logger)
        {
            this.logger = logger;
            this.domain = domain;
        }
        public IActionResult Index()
        {
            logger.LogTrace("Trace");
            logger.LogDebug("Debug");
            logger.LogInformation("Information");
            logger.LogWarning("Warning");
            logger.LogError(new Exception(), "Error");
            logger.LogCritical(new Exception(), "Critial");

            domain.Foo();
            return View();
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
