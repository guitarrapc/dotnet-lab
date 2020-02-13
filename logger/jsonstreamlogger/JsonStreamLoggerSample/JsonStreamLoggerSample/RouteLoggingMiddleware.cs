using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace JsonStreamLoggerSample
{
    public class RouteLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RouteLoggingMiddleware> _logger;

        public RouteLoggingMiddleware(RequestDelegate next, ILogger<RouteLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            finally
            {
                _logger.LogInformation($"{context.Response.StatusCode}\t{context.Request.Host}\t{context.Request.Method}\t{context.Request.Path}");
            }
        }
    }

    public static class RouteLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRouteLogging(this IApplicationBuilder builder)
            => builder.UseMiddleware<RouteLoggingMiddleware>();
    }
}
