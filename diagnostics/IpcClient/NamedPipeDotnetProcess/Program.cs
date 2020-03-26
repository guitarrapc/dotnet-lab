using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona;
using DiagnosticCore;

namespace NamedPipeProcess
{
    class Program
    {
        static async Task Main(string[] args)
            => await CoconaLiteApp.RunAsync<Diagnostics>(args);
    }

    public class Diagnostics
    {
        public void Ps()
        {
            var current = Process.GetCurrentProcess().Id;
            var processes = DiagnosticClient.GetAttachableProcesses()
                .Select(x => (isSelf: x.Id == current, ps: x))
                .ToArray();

            var sb = new StringBuilder();
            foreach (var (isSelf, process) in processes)
            {
                var self = isSelf ? "* " : "  ";

                try
                {
                    sb.Append($"{self} {process.Id,10} {process.ProcessName,-10} {process.MainModule.FileName}\n");
                }
                catch (InvalidOperationException)
                {
                    sb.Append($"{self} {process.Id,10} {process.ProcessName,-10} [Elevated process - cannot determine path]\n");
                }
                catch (NullReferenceException)
                {
                    sb.Append($"{self} {process.Id,10} {process.ProcessName,-10} [Elevated process - cannot determine path]\n");
                }
            }
            Console.WriteLine(sb.ToString());
        }

    }
}
