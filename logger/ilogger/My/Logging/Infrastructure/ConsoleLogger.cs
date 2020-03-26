using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace My.Logging.Infrastructure
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ConsoleLogger> cache = new ConcurrentDictionary<string, ConsoleLogger>();
        public ILogger CreateLogger(string categoryName)
        {
            return cache.GetOrAdd(categoryName, categoryName => new ConsoleLogger(categoryName));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class ConsoleLogger : ILogger
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly string _categoryName;
#pragma warning restore IDE0052 // Remove unread private members
        public ConsoleLogger(string categoryName)
            => _categoryName = categoryName;

        public IDisposable BeginScope<TState>(TState state)
        {
            return EmptyDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (state is My.Logging.Infrastructure.LogEntryPayload<HogeLogEntryMetadata, HogeLogEntryState> payload)
            {
                Console.WriteLine($"{DateTime.Now.ToString("o")} | {formatter(state, exception)} (Id:{payload.Metadata.Id})");
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
    }

    public class EmptyDisposable : IDisposable
    {
        public static IDisposable Instance { get; } = new EmptyDisposable();
        private EmptyDisposable() { }
        public void Dispose() { }
    }
}

namespace Microsoft.Extensions.Logging
{
    using My.Logging.Infrastructure;

    public static class ConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddConsoleLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, ConsoleLoggerProvider>(_ => new ConsoleLoggerProvider());

            return builder;
        }
    }
}
