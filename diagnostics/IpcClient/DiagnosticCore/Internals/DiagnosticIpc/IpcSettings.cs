using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticCore.Internals.DiagnosticIpc
{
    internal static class IpcSettings
    {
        /// <summary>
        /// Magic keyword to determine which process can be attach.
        /// </summary>
        public static string PipeName { get; } = Environment.GetEnvironmentVariable("DIAGNOSTIC_CLIENT_NAMED_PIPE_NAME") ?? $"dotnet-diagnostic-"; //$"diagnostic-client-";
    }
}
