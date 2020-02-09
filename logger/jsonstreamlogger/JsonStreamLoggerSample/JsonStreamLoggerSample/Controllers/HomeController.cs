using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JsonStreamLoggerSample.Models;

namespace JsonStreamLoggerSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation(@"SELECT * FROM HOGE WHERE ITEM = ""hoge's hogemoge."";");
            // !\u0022#$%\u0026\u0027()=~|-^\\@[\u0060{;:]\u002B*},./\u003C\u003E?_
            _logger.LogInformation(@"This is Ascii Keywords: !""#$%&'()=~|-^\@[`{;:]+*},./<>?_");
            _logger.LogInformation(@"This is Hiragana : ひらがな");
            _logger.LogInformation(@"This is Katakana : カタカナ");
            _logger.LogInformation(@"This is Kanji : 漢字");
            _logger.LogInformation(@"This is Sarrogate Kanji : 𩸽");
            _logger.LogError(new ArgumentException("hogemoge"), "exception!!!");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
