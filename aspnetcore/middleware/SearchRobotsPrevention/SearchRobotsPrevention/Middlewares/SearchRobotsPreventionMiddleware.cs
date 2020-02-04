using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SearchRobotsPrevention.Middlewares
{
    public static class SearchRobotsPreventionMiddlewareExtensions
    {
        public static IApplicationBuilder UseSearchRobotsPrevention(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SearchRobotsPreventionMiddleware>();
        }
    }

    public class SearchRobotsPreventionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _maxAge = $"max-age={TimeSpan.FromDays(1).TotalSeconds}";

        public SearchRobotsPreventionMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/robots.txt"))
            {
                context.Response.ContentType = "text/plain";
                context.Response.Headers.Add("Cache-Control", _maxAge);

                await context.Response.WriteAsync("User-Agent: *\r\nDisallow: /");
            }
            else
            {
                await this._next(context);
            }
        }
    }
}
