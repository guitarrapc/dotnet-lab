using JsonStreamLogger;
using JsonStreamLogger.Serialization;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JsonStreamLoggerSample
{
    public class SimpleWriter : IEntryWriter
    {
        private readonly Stream _stream;
        private readonly StreamWriter _writer;
        private static readonly byte[] _newLine = new byte[] { 0x0a /* \n */ };
        private const string OriginalFormatKey = "{OriginalFormat}";

        public SimpleWriter(Stream stream)
        {
            _stream = stream;
            _writer = new StreamWriter(stream, new UTF8Encoding(false));
        }

        public ValueTask WriteEntryAsync(in LogEntry entry, CancellationToken cancellationToken)
        {
            WriteLogEntry(_writer, entry);

            // Flush and reset internal buffer in Utf8JsonWriter here.
            _writer.Flush();
            _stream.Write(_newLine, 0, 1);
            _writer.Flush();

            return new ValueTask(_stream.FlushAsync(cancellationToken));
        }

        private static void WriteLogEntry(StreamWriter writer, in LogEntry entry)
        {
            writer.Write($"LogLevel: {(int)entry.LogLevel}" + "; ");
            WriteException(writer, entry.Exception);
            writer.Write($"Message: {entry.Message}" + "; ");
        }

        private static void WriteException(StreamWriter writer, Exception? ex)
        {
            if (ex == null)
            {
                return;
            }
            else
            {
                writer.Write("Exception: " + "{");
                {
                    writer.Write("Name: ", ex.GetType().FullName + ";");
                    writer.Write("Message: ", ex.Message + ";");
                    writer.Write("StackTrace: ", ex.StackTrace + ";");
                    writer.Write("InnerException: " + "{");
                    {
                        WriteException(writer, ex.InnerException);
                    }
                    writer.Write("}");
                }
                writer.Write("}" + "; ");
            }
        }
    }
}
