using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Http2Server
{
    public class RequestDiagnosticMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestDiagnosticMiddleware> _logger;

        public RequestDiagnosticMiddleware(RequestDelegate next, ILogger<RequestDiagnosticMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation($"Host: {context.Request.Host}");
            _logger.LogInformation($"Scheme: {context.Request.Scheme}");
            _logger.LogInformation($"Method: {context.Request.Method}");
            _logger.LogInformation($"Path: {context.Request.Path}");
            _logger.LogInformation($"Protocol: {context.Request.Protocol}");
            _logger.LogInformation("Headers:");
            foreach (var header in context.Request.Headers)
            {
                _logger.LogInformation($"  {header.Key}:{header.Value}");
            }

            await _next(context);
        }
    }

    public static class RequestDiagnosticMiddlewareMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestDiagnostic(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestDiagnosticMiddleware>();
        }
    }
}
