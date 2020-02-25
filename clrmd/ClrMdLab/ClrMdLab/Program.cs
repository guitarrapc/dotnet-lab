using System;
using System.IO;
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

    partial class ClrmdRun
    {
        public async Task Run([Option('i', Description = "Input dumpfile to run.")] string input)
        {
            if (!File.Exists(input))
                throw new FileNotFoundException($"could not find {input}, make sure dump file exsists.");

            var reader = new ClrmdReader();

            // ## HANDLE DUMP
            // REF: https://github.com/microsoft/clrmd/blob/master/doc/GettingStarted.md
            // **load existing dump**
            using var dataTarget = DataTarget.LoadCrashDump(input);

            // **attach to live process** (CLRMD core only support Passive)
            //using var attachLiveProcess = DataTarget.AttachToProcess(39744, 5000, AttachFlag.Passive);
            //attachLiveProcess.Dump();

            // show clr and dac information
            reader.ShowClrInfo(dataTarget);
            await Task.Delay(TimeSpan.FromSeconds(3));

            // ready ClrRuntime
            var runtime = dataTarget.ClrVersions[0].CreateRuntime();
            await Task.Delay(TimeSpan.FromSeconds(3));

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
            await Task.Delay(TimeSpan.FromSeconds(3));
            reader.ShowHeapProcessorAffinity(runtime);
            await Task.Delay(TimeSpan.FromSeconds(3));

            //WalkManagedObject(runtime);
            //WalkManagedObject2(runtime);

            // ## CLR Type and Fields
            // REF: https://github.com/microsoft/clrmd/blob/master/doc/TypesAndFields.md
            //WalkManagedObjectEx(runtime, ShowBasicType, ShowFieldValue);
            reader.WalkManagedObjectEx(runtime, null, reader.ShowFieldValue);
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}
