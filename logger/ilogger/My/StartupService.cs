using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using My.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace My
{
    public class StartupService : IHostedService
    {
        public StartupService(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            MyLogger.SetUnderlyingLogger(loggerFactory.CreateLogger("My"));
            RegisterUnhandledExceptionLogging();
            MyEnvironment.Initialize(configuration);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void RegisterUnhandledExceptionLogging()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    MyLogger.Current.UnhandledException(ex);
                }
            };
        }
    }

    public static class MyEnvironment
    {
        public static IConfiguration Configuration { get; private set; } = default!;

        public static bool IsLocal { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            Configuration = configuration;

            IsLocal = Configuration.GetSection("My:IsLocal").Get<bool>();
        }
    }
}
