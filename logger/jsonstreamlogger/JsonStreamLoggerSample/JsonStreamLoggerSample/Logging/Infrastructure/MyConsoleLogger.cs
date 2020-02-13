using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsonStreamLoggerSample.Logging.Infrastructure
{
    public class MyConsoleLogger : ILogger
    {
        private readonly string _categoryName;

        public MyConsoleLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (state is MyClass c)
            {
                Console.WriteLine($"{DateTime.Now.ToString("o")} | {formatter(state, exception)} (Name:{c.Name})");
            }
            else
            {
                Console.WriteLine($"{DateTime.Now.ToString("o")} | {formatter(state, exception)}");
            }

            if (exception != null)
            {
                Console.WriteLine(exception);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return AnonymousDisposable.Instance;
        }

        private class AnonymousDisposable : IDisposable
        {
            public static IDisposable Instance { get; } = new AnonymousDisposable();

            public void Dispose()
            {
            }
        }
    }

    [ProviderAlias("MyConsole")]
    public class MyConsoleLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MyConsoleLogger(categoryName);
        }
    }

    public class MyClass
    {
        public string Name { get; set; }
    }
}


namespace Microsoft.Extensions.Logging
{
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// Register Console output logger. Only used for Local Debug
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddMyConsoleLogger(this ILoggingBuilder builder)
        {
            return builder.AddProvider(new JsonStreamLoggerSample.Logging.Infrastructure.MyConsoleLoggerProvider());
        }
    }
}
