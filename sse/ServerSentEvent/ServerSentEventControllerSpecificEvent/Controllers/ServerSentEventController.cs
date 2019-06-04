using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ServerSentEvent.Controllers
{
    /// <summary>
    /// ServerSentEvent provided as Controller
    /// </summary>
    [Route("sse")]
    public class ServerSentEventController : ControllerBase
    {
        private static ConcurrentDictionary<string, Question[]> answers = new ConcurrentDictionary<string, Question[]>();

        private class Question
        {
            public DateTime Date => DateTime.Now;
            public int Id { get; set; }
            public string Message { get; set; }
            public string Answer { get; set; }
            public string Description { get; set; }
        }

        public ServerSentEventController()
        {
            answers.TryAdd("hogemoge", new[] {
                new Question()
                {
                    Id = 0,
                    Message = "Your question",
                    Answer = "Answer dayo!",
                    Description = "it is description."
                },
            });
            answers.TryAdd("fugafuga", new[] {
                new Question()
                {
                    Id = 0,
                    Message = "Your question Fuga",
                    Answer = "Answer dayo Fuga!",
                    Description = "it is description Fuga."
                },
            });
        }

        [HttpGet]
        public async Task Get()
        {
            var response = Response;
            response.Headers.Add("Content-Type", "text/event-stream");

            var user = Request.Query.Where(x => x.Key == "username").Select(x => x.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(user))
            {
                await response.WriteAsync($"event:\n");
                await response.WriteAsync($"data: Missing query `?username=xxxx` \n\n");
                return;
            }

            for (var i = 0; true; ++i)
            {
                answers.TryGetValue(user, out var value);
                var res = JsonConvert.SerializeObject(value);
                await response.WriteAsync($"event:{user}\n");
                await response.WriteAsync($"data: {res}\n\n");

                await response.Body.FlushAsync();
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
