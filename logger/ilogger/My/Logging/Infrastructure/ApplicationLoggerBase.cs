using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace My.Logging.Infrastructure
{
    // base class for application logger
    public abstract class ApplicationLoggerBase<TMetadata, TStateBase> : IApplicationLogger
        where TMetadata : struct
    {
        protected ILogger Logger { get; }

        protected ApplicationLoggerBase(ILogger logger)
            => Logger = logger;

        protected void Log(LogLevel logLevel, EventId eventId, Func<TMetadata, TStateBase, Exception, string> formatter)
            => Log<TStateBase>(logLevel, eventId, default!, formatter);

        protected virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Func<TMetadata, TState, Exception, string> formatter)
            where TState : TStateBase
            => Logger.Log(
                logLevel,
                eventId,
                new LogEntryPayload<TMetadata, TStateBase>(GetMetadata(), state),
                null,
                (arg0, exception0) => Format(arg0.Metadata, (TState)arg0.Detail!, exception0, formatter)
            );

        protected virtual void LogException<TState>(LogLevel logLevel, EventId eventId, Exception exception, TState state, Func<TMetadata, TState, Exception, string> formatter)
            where TState : TStateBase
            => Logger.Log(
                logLevel,
                eventId,
                new LogEntryPayload<TMetadata, TStateBase>(GetMetadata(), state, exception),
                exception,
                (arg0, exception0) => Format(arg0.Metadata, (TState)arg0.Detail!, exception0, formatter)
            );

        protected void LogException(LogLevel logLevel, EventId eventId, Exception exception)
            => LogException<TStateBase>(logLevel, eventId, exception, default!);

        protected void LogException<TState>(LogLevel logLevel, EventId eventId, Exception exception, TState state)
            where TState : TStateBase
            => LogException(logLevel, eventId, exception, state, (arg1, arg2, exception1) => $"{exception1.GetType().FullName}: {exception1.Message}");

        protected abstract TMetadata GetMetadata();

        protected virtual string Format<TState>(TMetadata metadata, TState state, Exception exception, Func<TMetadata, TState, Exception, string> formatter)
            => formatter(metadata, state, exception);
    }
}
