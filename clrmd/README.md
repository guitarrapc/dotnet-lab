## CLRMD

dump load and programically run SOS from C#.

You have several choice to get dump.

* create dump with `dotnet-dump` and load file.
* create dump and attach to process.
* attach to process.

Make sure you can not load linux dump on Windows.

* Windows dump can load on windows, but not on Linux.
* Linux dump can load on Linux, but not on Windows.

## How to

unzip `core_MemoryLeak.WorkstationGC.202002181232.dump.zip` and run.

```shell
dotnet run - run -i ./core_MemoryLeak.WorkstationGC.202002181232.dump
```

## REF

> [mattwarren/HeapStringAnalyser: Analyse Memory Dumps looking at \.NET String types](https://github.com/mattwarren/HeapStringAnalyser)
> [DataDog/dd-trace-dotnet](https://github.com/DataDog/dd-trace-dotnet/blob/e481fc79c5742f9870b216ab6f40c81345ca95f6/src/Datadog.Trace.ClrProfiler.Native/clr_helpers.cpp)
