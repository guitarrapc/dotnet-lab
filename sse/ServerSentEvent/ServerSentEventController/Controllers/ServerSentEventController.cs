using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServerSentEvent.Controllers
{
    /// <summary>
    /// ServerSentEvent provided as Controller
    /// </summary>
    [Route("sse")]
    public class ServerSentEventController : ControllerBase
    {
        [HttpGet]
        public async Task Get()
        {
            var response = Response;
            response.Headers.Add("Content-Type", "text/event-stream");
            for (var i = 0; true; ++i)
            {
                await response.WriteAsync($"data: Controller {i} at {DateTime.Now}\n\n");

                await response.Body.FlushAsync();
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
