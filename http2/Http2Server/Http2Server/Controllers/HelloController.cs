using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Http2Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        private readonly ILogger<HelloController> _logger;

        public HelloController(ILogger<HelloController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Hello Get()
        {
            return new Hello
            {
                Host = Request.Host.Host,
                Scheme = Request.Scheme,
                Protocol = Request.Protocol,
                HostName = Environment.MachineName,
            };
        }
    }
}
