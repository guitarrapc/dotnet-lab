using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerSentEvent.Middlewares
{
    public class SseMiddleWare
    {
        private readonly RequestDelegate _next;
        public SseMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if(httpContext.Request.Path.ToString().Equals("/sse"))
            {
                var response = httpContext.Response;
                response.Headers.Add("Content-Type", "text/event-stream");

                for (var i = 0; true; ++i)
                {
                    await response.WriteAsync($"data: Middleware {i} at {DateTime.Now}\n\n");

                    await response.Body.FlushAsync();
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            await _next.Invoke(httpContext);
        }
    }

    public static class SseMiddleWareExtensions
    {
        public static IApplicationBuilder UseSse(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SseMiddleWare>();
        }
    }
}
