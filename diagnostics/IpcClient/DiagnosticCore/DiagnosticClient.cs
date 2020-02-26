using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DiagnosticCore.Internals.DiagnosticIpc;

namespace DiagnosticCore
{
    public class DiagnosticClient
    {
        public static IEnumerable<int> GetPublishedProcessIds()
            => Directory.GetFiles(IpcClient.IpcRootPath)
                    .Select(x => new FileInfo(x).Name)
                    .Where(x => Regex.IsMatch(x, IpcClient.DiagnosticsPortPattern))
                    .Select(x => int.Parse(Regex.Match(x, IpcClient.DiagnosticsPortPattern).Groups[1].Value, NumberStyles.Integer))
                    .Distinct();

        public static IEnumerable<Process> GetAttachableProcesses()
            => DiagnosticClient.GetPublishedProcessIds()
                .Select(x => GetProcessById(x))
                .Where(x => x != null)
                .OrderBy(x => x.ProcessName)
                .ThenBy(x => x.Id);

        private static Process GetProcessById(int processId)
        {
            try
            {
                return Process.GetProcessById(processId);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
