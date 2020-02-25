using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Cocona;
using Microsoft.Diagnostics.Runtime;

namespace ClrMdLab
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CoconaLiteApp.RunAsync<ClrmdRun>(args);
        }
    }

    partial class ClrmdRun : CoconaLiteConsoleAppBase
    {
        public async Task File([Option('i', Description = "Input dumpfile to run.")] string input, bool showobj = false)
        {
            if (!System.IO.File.Exists(input))
                throw new FileNotFoundException($"could not find {input}, make sure dump file exsists.");

            // ## HANDLE DUMP
            // REF: https://github.com/microsoft/clrmd/blob/master/doc/GettingStarted.md
            // **load existing dump**
            using var dataTarget = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? DataTarget.LoadCrashDump(input, CrashDumpReader.ClrMD)
                : DataTarget.LoadCoreDump(input);

            await Core(dataTarget, showobj);
        }

        public async Task Process([Option('i', Description = "Input process id to run.")] int pid, bool showobj = false)
        {
            // ## HANDLE DUMP
            // REF: https://github.com/microsoft/clrmd/blob/master/doc/GettingStarted.md
            // **load existing dump**
            using var dataTarget = DataTarget.AttachToProcess(pid, 5000, AttachFlag.Passive);

            await Core(dataTarget, showobj);
        }

        private async Task Core(DataTarget dataTarget, bool showobj)
        {
            var reader = new ClrmdReader();

            // **attach to live process** (CLRMD core only support Passive)
            //using var attachLiveProcess = DataTarget.AttachToProcess(39744, 5000, AttachFlag.Passive);
            //attachLiveProcess.Dump();

            // show clr and dac information
            reader.ShowClrInfo(dataTarget);
            await Task.Delay(TimeSpan.FromSeconds(3), Context.CancellationToken);

            // ready ClrRuntime
            var runtime = dataTarget.ClrVersions[0].CreateRuntime();
            await Task.Delay(TimeSpan.FromSeconds(3), Context.CancellationToken);

            // set custom symbol server by env `_NT_SYMBOL_PATH` or use Microsoft Symbol Server.

            // ## HANDLE RUNTIME
            // REF: https://github.com/microsoft/clrmd/blob/master/doc/ClrRuntime.md
            reader.ShowClrRuntimeInfo(runtime);
            await Task.Delay(TimeSpan.FromSeconds(3));
            reader.ShowAppDomainInfo(runtime);
            await Task.Delay(TimeSpan.FromSeconds(3));
            reader.ShowStacks(runtime);
            await Task.Delay(TimeSpan.FromSeconds(3));
            reader.ShowMemoryRegion(runtime);
            await Task.Delay(TimeSpan.FromSeconds(3));
            reader.ShowFinalizerQueue(runtime);
            await Task.Delay(TimeSpan.FromSeconds(3));
            reader.ShowGcHandles(runtime);
            await Task.Delay(TimeSpan.FromSeconds(3));

            // ## HANDLE GC HEAP
            // REF: https://github.com/microsoft/clrmd/blob/master/doc/WalkingTheHeap.md
            // SERVER GC | WORKSTATION GC
            // ---- | ----
            // multiple threads | only 1 thread
            // many logical heap | 1 logical heap

            reader.ShowHeap(runtime);
            await Task.Delay(TimeSpan.FromSeconds(3), Context.CancellationToken);
            reader.ShowHeapProcessorAffinity(runtime);

            //WalkManagedObject(runtime);
            //WalkManagedObject2(runtime);

            if (showobj)
            {
                await Task.Delay(TimeSpan.FromSeconds(3), Context.CancellationToken);

                // ## CLR Type and Fields
                // REF: https://github.com/microsoft/clrmd/blob/master/doc/TypesAndFields.md
                //WalkManagedObjectEx(runtime, ShowBasicType, ShowFieldValue);
                reader.WalkManagedObjectEx(runtime, null, reader.ShowFieldValue);
                await Task.Delay(TimeSpan.FromSeconds(3), Context.CancellationToken);
            }
        }
    }
}
