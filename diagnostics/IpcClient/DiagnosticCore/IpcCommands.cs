using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticCore
{
    internal enum DiagnosticsServerCommandSet : byte
    {
        Dump = 0x01,
        EventPipe = 0x02,
        Profiler = 0x03,

        Server = 0xFF,
    }

    internal enum DiagnosticsServerCommandId : byte
    {
        OK = 0x00,
        Error = 0xFF,
    }

    internal enum EventPipeCommandId : byte
    {
        StopTracing = 0x01,
        CollectTracing = 0x02,
        CollectTracing2 = 0x03,
    }

    internal enum DumpCommandId : byte
    {
        GenerateCoreDump = 0x01,
    }

    internal enum ProfilerCommandId : byte
    {
        AttachProfiler = 0x01,
    }
}
