using Microsoft.Extensions.Logging;
using My.Logging.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace My.Logging
{
    public class HogeStoredLogEntry
    {
        public DateTimeOffset Timestamp { get; set; }
        public LogLevel LogLevel { get; set; }
        public EventId EventId { get; set; }
        public LogEntryPayload<HogeLogEntryMetadata, HogeLogEntryState> State { get; set; }
        public ExceptionInfo? Exception { get; set; }
        public string Message { get; set; }

        public HogeStoredLogEntry()
        {
            Timestamp = default;
            LogLevel = default;
            EventId = default;
            State = default!;
            Exception = default;
            Message = default!;
        }

        public HogeStoredLogEntry(LogLevel logLevel, EventId eventId, LogEntryPayload<HogeLogEntryMetadata, HogeLogEntryState> state, Exception? exception, string message)
        {
            Timestamp = DateTimeOffset.Now;
            LogLevel = logLevel;
            EventId = eventId;
            State = state;
            Exception = (exception != null) ? new ExceptionInfo(exception) : null;
            Message = message;
        }
    }

    class LoggerBuffer : ILogger
    {
        private readonly ILogger _logger;
        private readonly ConcurrentQueue<HogeStoredLogEntry> _buffer;

        public IReadOnlyList<HogeStoredLogEntry> LogEntries => _buffer.ToArray();

        public LoggerBuffer(ILogger logger)
        {
            _logger = logger;
            _buffer = new ConcurrentQueue<HogeStoredLogEntry>();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log<TState>(logLevel, eventId, state, exception, formatter);

            // Do log store LogEntry which occur exception during logging 
            if (eventId == HogeLogEvent.UnhandledExceptionInLogging) return;

            if (_logger.IsEnabled(logLevel))
            {
                _buffer.Enqueue(new HogeStoredLogEntry(logLevel, eventId, (LogEntryPayload<HogeLogEntryMetadata, HogeLogEntryState>)(object)state!, exception, formatter(state, exception)));
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return EmptyDisposable.Instance;
        }
    }
}
