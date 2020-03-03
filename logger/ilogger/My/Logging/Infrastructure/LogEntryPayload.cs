using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace My.Logging.Infrastructure
{
    public struct LogEntryPayload<TMetadata, TDetail> 
        where TMetadata : struct
    {
        public TMetadata Metadata { get; set; }
        public TDetail Detail { get; set; }
        public ExceptionInfo? Exception { get; set; }

        public LogEntryPayload(TMetadata metadata, TDetail detail)
        {
            Metadata = metadata;
            Detail = detail;
            Exception = default;
        }

        public LogEntryPayload(TMetadata metadata, TDetail detail, Exception exception)
        {
            Metadata = metadata;
            Detail = detail;
            Exception = (exception != null) ? new ExceptionInfo(exception) : null;
        }
    }

    public class ExceptionInfo
    {
        public string TypeName { get; set; }
        public string Message { get; set; }
        public string? StackTrace { get; set; }
        public string? Source { get; set; }
        public ExceptionInfo? InnerException { get; set; }

        public ExceptionInfo()
        {
            TypeName = string.Empty;
            Message = string.Empty;
            StackTrace = null;
            Source = null;
            InnerException = null;
        }

        public ExceptionInfo(Exception exception)
        {
            TypeName = exception.GetType().FullName!;
            Message = exception.Message;
            StackTrace = exception.StackTrace;
            Source = exception.Source;
            InnerException = (exception.InnerException != null) ? new ExceptionInfo(exception.InnerException) : null;
        }
    }
}
