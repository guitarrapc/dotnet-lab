using Microsoft.Extensions.Logging;

namespace My.Logging
{
    public static class MyServerLogEvent
    {
        public static readonly EventId UnhandledException = new EventId(11900, nameof(UnhandledException));
        public static readonly EventId UnhandledExceptionInForget = new EventId(11901, nameof(UnhandledExceptionInForget));
        public static readonly EventId GeneralWarning = new EventId(11902, nameof(GeneralWarning));
        public static readonly EventId GeneralError = new EventId(11903, nameof(GeneralError));
        public static readonly EventId GeneralInformation = new EventId(11904, nameof(GeneralInformation));
        public static readonly EventId Debug = new EventId(11999, nameof(Debug));
    }

    public static class HogeLogEvent
    {
        public static readonly EventId Connect = new EventId(10000, nameof(Connect));

        public static readonly EventId UnhandledException = new EventId(10900, nameof(UnhandledException));
        public static readonly EventId UnhandledExceptionInLogging = new EventId(10901, nameof(UnhandledExceptionInLogging));
        public static readonly EventId Debug = new EventId(10999, nameof(Debug));
    }
}
