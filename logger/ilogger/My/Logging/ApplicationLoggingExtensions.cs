using Microsoft.Extensions.DependencyInjection;
using My.Logging.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging
{
    public static class ApplicationLoggingExtensions
    {
        public static IServiceCollection AddApplicationLoggers(this IServiceCollection services)
        {
            var allConcreteTypes = Assembly.GetEntryAssembly()!.GetTypes().Where(x => !x.IsAbstract).ToArray();

            // Register ApplicationLogger
            foreach (var loggerType in allConcreteTypes.Where(x => typeof(ISingletonApplicationLogger).IsAssignableFrom(x) && typeof(ISingletonApplicationLogger) != x))
            {
                services.AddSingleton(loggerType);
            }

            return services;
        }
    }
}
