using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using My.Logging;
using My.Models;

namespace My.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HogeContext context;

        public HomeController(ILogger<HomeController> logger, ILogger<HogeContextLogger> contextLogger)
        {
            _logger = logger;
            context = new HogeContext(Guid.NewGuid(), contextLogger);
        }

        public IActionResult Index()
        {
            context.Logger.Connect(1000);
            context.Logger.Exception(new Exception("exception message"));
            context.Logger.Debug("this is debug message");
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
