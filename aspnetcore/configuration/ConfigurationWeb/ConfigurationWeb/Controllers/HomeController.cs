using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConfigurationWeb.Models;
using Microsoft.Extensions.Configuration;

namespace ConfigurationWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            // environment
            _logger.LogInformation($"{_config.GetSection("ENV_HOGE").Exists()}, {_config.GetSection("ENV_HOGE").Value}"); // `FALSE,`
            _logger.LogInformation($"{_config.GetValue("ENV_HOGE", "default")}"); // `hoge`
            _logger.LogInformation($"{_config.GetValue("ENV_FUGA", "default")}"); // `default`
            // appsettings
            _logger.LogInformation($"{_config.GetSection("AllowedHosts").Exists()}, {_config.GetSection("AllowedHosts").Value}"); // `True, *`
            _logger.LogInformation($"{_config.GetSection("Logging__LogLevel__Default").Exists()}, {_config.GetSection("Logging__LogLevel__Default").Value}"); // `False, `
            _logger.LogInformation($"{_config.GetSection("Logging__LogLevel").Exists()}, {_config.GetSection("Logging__LogLevel").GetValue<string>("Default")}"); // `False, `
            _logger.LogInformation($"{_config.GetSection("Logging:LogLevel:Default").Exists()}, {_config.GetSection("Logging:LogLevel:Default").Value}"); // `True, Information`
            _logger.LogInformation($"{_config.GetSection("Logging:LogLevel").Exists()}, {_config.GetSection("Logging:LogLevel").GetValue<string>("Default")}"); // `True, Information`
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
