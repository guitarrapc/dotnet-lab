using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace My.Logging
{
    public class MyLogger
    {
        private ILogger? _logger;

        public static MyLogger Current { get; } = new MyLogger();

        public static void SetUnderlyingLogger(ILogger logger)
        {
            Current._logger = logger;
        }

        public void UnhandledException(Exception ex)
        {
            _logger?.Log(LogLevel.Error, MyServerLogEvent.UnhandledException, ex, "Unhandled Exception: " + ex.Message);
        }

        public void UnhandledExceptionInForget(Exception ex)
        {
            _logger?.Log(LogLevel.Warning, MyServerLogEvent.UnhandledExceptionInForget, ex, "Unhandled Exception: " + ex.Message);
        }

        public void Debug(string message)
        {
            _logger?.Log(LogLevel.Debug, MyServerLogEvent.Debug, message);
        }

        public void Warning(string message, Exception? ex = null)
        {
            _logger?.Log(LogLevel.Warning, MyServerLogEvent.GeneralWarning, message, ex);
        }

        public void Error(string message, Exception? ex = null)
        {
            _logger?.Log(LogLevel.Error, MyServerLogEvent.GeneralError, message, ex);
        }

        public void Information(string message)
        {
            _logger?.Log(LogLevel.Information, MyServerLogEvent.GeneralInformation, message);
        }
    }
}
