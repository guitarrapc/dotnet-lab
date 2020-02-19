using Microsoft.Extensions.Logging;
using My.Logging.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace My.Logging
{
    public class HogeContext
    {
        public Guid Id { get; set; }
        public HogeContextLogger Logger { get; set; }

        public HogeContext(Guid id, ILogger<HogeContextLogger> logger)
        {
            Id = id;
            Logger = new HogeContextLogger(logger, this);

            // other properties...
        }
    }


    public partial class HogeContextLogger : ApplicationLoggerBase<HogeLogEntryMetadata, HogeLogEntryState>
    {
        private readonly HogeContext _context;
#pragma warning disable IDE0052 // Remove unread private members
        private readonly LoggerBuffer _loggerBuffer;
#pragma warning restore IDE0052 // Remove unread private members

        public HogeContextLogger(ILogger<HogeContextLogger> logger, HogeContext context)
            : this(new LoggerBuffer(logger), context)
        {
        }

        private HogeContextLogger(LoggerBuffer logger, HogeContext context)
            : base(logger)
        {
            _context = context;
            _loggerBuffer = logger;
        }

        protected override HogeLogEntryMetadata GetMetadata()
            => new HogeLogEntryMetadata(_context.Id);

        public void Connect(int uid)
            => Log(LogLevel.Information, HogeLogEvent.Connect, new HogeLogEntryStateConnect(uid), (metadata, state, _) => $"Connect: {state.Uid}");

        public void Exception(Exception exception, int? uid = null, string? accessPath = null, int? connectId = null)
            => LogException(LogLevel.Error, HogeLogEvent.UnhandledException, exception, new HogeLogEntryStateUnhandledException(uid, accessPath, connectId));

        // May be better down LogLevel to LogLevel.Debug
        public void Debug(string message)
            => Log(LogLevel.Information, HogeLogEvent.Debug, new HogeLogEntryStateDebug(message), (metadata, detail, _) => $"Debug: {detail.Message}");
    }

    public readonly struct HogeLogEntryMetadata
    {
        public Guid Id { get; }
        public HogeLogEntryMetadata(Guid id)
        {
            Id = id;
        }
    }
}
