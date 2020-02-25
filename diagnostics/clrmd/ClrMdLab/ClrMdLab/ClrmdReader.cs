using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Diagnostics.Runtime;

namespace ClrMdLab
{
    class ClrmdReader
    {
        // OUTPUT SAMPLE
        // ------------------
        //Found CLR Version: v4.700.19.56402
        //Flavor: Core
        //Filesize:  56C000
        //Timestamp: 5DCDF95D
        //Dac File: mscordaccore_Amd64_Amd64_4.700.19.56402.dll
        //Local dac location: C:\Program Files\dotnet\shared\Microsoft.NETCore.App\3.1.0\mscordaccore.dll
        // ------------------
        public void ShowClrInfo(DataTarget dataTarget)
        {
            foreach (ClrInfo version in dataTarget.ClrVersions)
            {
                Console.WriteLine("Found CLR Version: " + version.Version);
                Console.WriteLine("Flavor: " + version.Flavor);

                // This is the data needed to request the dac from the symbol server:
                ModuleInfo dacInfo = version.DacInfo;
                Console.WriteLine("Filesize:  {0}", dacInfo.FileSize);
                Console.WriteLine("Timestamp: {0}", DateTimeOffset.FromUnixTimeSeconds(dacInfo.TimeStamp));
                Console.WriteLine("Dac File:  {0}", dacInfo.FileName);

                // If we just happen to have the correct dac file installed on the machine,
                // the "LocalMatchingDac" property will return its location on disk:
                string dacLocation = version.LocalMatchingDac;
                if (!string.IsNullOrEmpty(dacLocation))
                    Console.WriteLine("Local dac location: " + dacLocation);

                // You may also download the dac from the symbol server
                Console.WriteLine();
            }
        }

        // OUTPUT SAMPLE
        // ------------------
        //Is ServerGC: False
        //Is WorkstationGC: True
        //HeapCount: 1
        //TotalHeapSize: 5104648
        // ------------------
        public void ShowClrRuntimeInfo(ClrRuntime runtime)
        {
            Console.WriteLine("Is ServerGC: " + runtime.ServerGC);
            Console.WriteLine("Is WorkstationGC: " + !runtime.ServerGC);

            Console.WriteLine("HeapCount: " + runtime.HeapCount);
            Console.WriteLine("TotalHeapSize: " + runtime.Heap.TotalHeapSize);

            Console.WriteLine();
        }

        // OUTPUT SAMPLE
        // ------------------
        //ID: 1
        //Name: clrhost
        //Address: 2207136258224
        // ------------------
        public void ShowAppDomainInfo(ClrRuntime runtime, bool showModules = false)
        {
            foreach (ClrAppDomain domain in runtime.AppDomains)
            {
                Console.WriteLine("ID:      {0}", domain.Id);
                Console.WriteLine("Name:    {0}", domain.Name);
                Console.WriteLine("Address: {0}", domain.Address);

                if (showModules)
                {
                    foreach (ClrModule module in domain.Modules)
                    {
                        Console.WriteLine("Module: {0}", module.Name);
                    }
                }

                Console.WriteLine();
            }
        }

        // OUTPUT SAMPLE
        // ------------------
        //Thread 6964:
        //  B63277DFB8            0[HelperMethodFrame]
        //  B63277E0C0 7FFEA33A7874 System.Threading.WaitHandle.WaitOneNoCheck(Int32)
        //  B63277E120 7FFEA33AA171 System.Threading.WaitHandle.WaitOne(Int32)
        //  B63277E160 7FFEC74D44B8 System.Diagnostics.Process.WaitForExitCore(Int32)
        // ------------------
        public void ShowStacks(ClrRuntime runtime)
        {
            foreach (ClrThread thread in runtime.Threads)
            {
                if (!thread.IsAlive)
                    continue;

                Console.WriteLine("Thread {0:X}:", thread.OSThreadId);

                foreach (ClrStackFrame frame in thread.StackTrace)
                    Console.WriteLine("{0,12:X} {1,12:X} {2}", frame.StackPointer, frame.InstructionPointer, frame);

                Console.WriteLine();
            }
        }

        // OUTPUT SAMPLE
        // ------------------
        //  2       12.915MB GCSegment
        //130        9.462MB LowFrequencyLoaderHeap
        //102        6.627MB HighFrequencyLoaderHeap
        //  2        0.713MB ResolveHeap
        //  2        0.401MB DispatchHeap
        //  2        0.074MB CacheEntryHeap
        //  2        0.049MB IndcellHeap
        //  2        0.033MB LookupHeap
        //  2        0.025MB StubHeap
        //  1        0.012MB HandleTableChunk
        // ------------------
        public void ShowMemoryRegion(ClrRuntime runtime)
        {
            foreach (var region in (from r in runtime.EnumerateMemoryRegions()
                                    where r.Type != ClrMemoryRegionType.ReservedGCSegment
                                    group r by r.Type into g
                                    let total = g.Sum(p => (uint)p.Size)
                                    orderby total descending
                                    select new
                                    {
                                        TotalSize = total,
                                        Count = g.Count(),
                                        Type = g.Key
                                    }))
            {
                Console.WriteLine("{0,6:n0} {1,12:n3}MB {2}", region.Count, region.TotalSize / 1000d / 1000d, region.Type);
            }
            Console.WriteLine();
        }

        public void ShowFinalizerQueue(ClrRuntime runtime)
        {
            var appDomain = runtime.AppDomains[0];
            foreach (ulong obj in runtime.EnumerateFinalizerQueueObjectAddresses())
            {
                var type = runtime.Heap.GetObjectType(obj);
                if (type == null)
                    continue;

                ulong size = type.GetSize(obj);
                Console.WriteLine("{0,12:X} {1,8:n0} {2}", obj, size, type.Name);
            }
            Console.WriteLine();
        }

        // OUTPUT SAMPLE
        // ------------------
        // 201E53A10D0  201E5561360 System.String System.String
        // 201E53A10D8  201E5561360 System.String System.String
        // 201E53A10E0  201E5561360 System.String System.String
        // 201E53A10E8  201E5561360 System.String System.String
        // ------------------
        public void ShowGcHandles(ClrRuntime runtime)
        {
            var heap = runtime.Heap;
            foreach (ClrHandle handle in runtime.EnumerateHandles())
            {
                string objectType = heap.GetObjectType(handle.Object).Name;
                Console.WriteLine("{0,12:X} {1,12:X} {2,12} {3}", handle.Address, handle.Object, handle.Type, objectType);
            }
            Console.WriteLine();
        }

        // OUTPUT SAMPLE
        // ------------------
        // Start End CommittedEnd ReservedEnd Heap Type
        // 201E5561000  201E5A109D0  201E6181000  201F5560000    0 Ephemeral
        // 201F5561000  201F558FA38  201F5592000  201FD560000    0 Large
        // ------------------
        public void ShowHeap(ClrRuntime runtime)
        {
            Console.WriteLine("{0,12} {1,12} {2,12} {3,12} {4,4} {5}", "Start", "End", "CommittedEnd", "ReservedEnd", "Heap", "Type");
            foreach (ClrSegment segment in runtime.Heap.Segments)
            {
                string type;
                if (segment.IsEphemeral)
                    type = "Ephemeral";
                else if (segment.IsLarge)
                    type = "Large";
                else
                    type = "Gen2";

                Console.WriteLine("{0,12:X} {1,12:X} {2,12:X} {3,12:X} {4,4} {5}", segment.Start, segment.End, segment.CommittedEnd, segment.ReservedEnd, segment.ProcessorAffinity, type);
            }
            Console.WriteLine();
        }

        // OUTPUT SAMPLE
        // ------------------
        // Heap  0 items, 5,104,648 bytes
        // ------------------
        public void ShowHeapProcessorAffinity(ClrRuntime runtime)
        {
            var heap = runtime.Heap;
            foreach (var item in (from seg in heap.Segments
                                  group seg by seg.ProcessorAffinity into g
                                  orderby g.Key
                                  select new
                                  {
                                      Heap = g.Key,
                                      Size = g.Sum(p => (uint)p.Length)
                                  }))
            {
                Console.WriteLine("Heap {0,2} items, {1:n0} bytes", item.Heap, item.Size);
            }
            Console.WriteLine();
        }

        public void WalkManagedObject(ClrRuntime runtime)
        {
            if (!runtime.Heap.CanWalkHeap)
            {
                Console.WriteLine("Cannot walk the heap!");
            }
            else
            {
                foreach (ClrSegment seg in runtime.Heap.Segments)
                {
                    for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
                    {
                        ClrType type = runtime.Heap.GetObjectType(obj);

                        // If heap corruption, continue past this object.
                        if (type == null)
                            continue;

                        ulong size = type.GetSize(obj);
                        Console.WriteLine("{0,12:X} {1,8:n0} {2,1:n0} {3}", obj, size, seg.GetGeneration(obj), type.Name);
                    }
                }
            }
            Console.WriteLine();
        }

        public void WalkManagedObject2(ClrRuntime runtime)
        {
            if (!runtime.Heap.CanWalkHeap)
            {
                Console.WriteLine("Cannot walk the heap!");
            }
            else
            {
                foreach (ulong obj in runtime.Heap.EnumerateObjectAddresses())
                {
                    ClrType type = runtime.Heap.GetObjectType(obj);

                    // If heap corruption, continue past this object.
                    if (type == null)
                        continue;

                    ulong size = type.GetSize(obj);
                    Console.WriteLine("{0,12:X} {1,8:n0} {2,1:n0} {3}", obj, size, runtime.Heap.GetGeneration(obj), type.Name);
                }
            }
            Console.WriteLine();
        }

        private static void ObjSize(ClrHeap heap, ulong obj, out uint count, out ulong size)
        {
            // Evaluation stack
            Stack<ulong> eval = new Stack<ulong>();

            // To make sure we don't count the same object twice, we'll keep a set of all objects
            // we've seen before.  Note the ObjectSet here is basically just "HashSet<ulong>".
            // However, HashSet<ulong> is *extremely* memory inefficient.  So we use our own to
            // avoid OOMs.
            ObjectSet considered = new ObjectSet(heap);

            count = 0;
            size = 0;
            eval.Push(obj);

            while (eval.Count > 0)
            {
                // Pop an object, ignore it if we've seen it before.
                obj = eval.Pop();
                if (considered.Contains(obj))
                    continue;

                considered.Add(obj);

                // Grab the type. We will only get null here in the case of heap corruption.
                ClrType type = heap.GetObjectType(obj);
                if (type == null)
                    continue;

                count++;
                size += type.GetSize(obj);

                // Now enumerate all objects that this object points to, add them to the
                // evaluation stack if we haven't seen them before.
                type.EnumerateRefsOfObject(obj, delegate (ulong child, int offset)
                {
                    if (child != 0 && !considered.Contains(child))
                        eval.Push(child);
                });
            }
        }

        // OUTPUT SAMPLE
        // ------------------
        //201E5561000       24 2 Free
        // 201E5561018      128 2 System.Exception
        //  Exception 0x201E5561018 hresult: 0x80131500
        //  - 2146233088
        //System.Exception
        //
        //null
        //80131500
        // 201E5561098      128 2 System.OutOfMemoryException
        //  Exception 0x201E5561098 hresult: 0x8007000E
        //  - 2147024882
        // ------------------
        public void WalkManagedObjectEx(ClrRuntime runtime, Action<ClrType> typeAction, Action<ClrType, ulong> fieldAction)
        {
            if (!runtime.Heap.CanWalkHeap)
            {
                Console.WriteLine("Cannot walk the heap!");
            }
            else
            {
                foreach (ulong obj in runtime.Heap.EnumerateObjectAddresses())
                {
                    ClrType type = runtime.Heap.GetObjectType(obj);

                    // If heap corruption, continue past this object.
                    if (type == null)
                        continue;
                    ulong size = type.GetSize(obj);
                    Console.WriteLine("{0,12:X} {1,8:n0} {2,1:n0} {3}", obj, size, runtime.Heap.GetGeneration(obj), type.Name);
                    typeAction?.Invoke(type);
                    ShowFieldValue(type, obj);
                    ReadThreadStaticVariables(runtime, type);
                }
            }
            Console.WriteLine();
        }

        public void ShowBasicType(ClrType type)
        {
            Console.WriteLine("  Type {0} implements interfaces:", type);

            foreach (var inter in type.Interfaces)
            {
                Console.WriteLine("    {0}", inter.Name);
            }
        }

        public void ShowFieldValue(ClrType type, ulong obj)
        {
            if (type.IsException)
            {
                ReadException(type, obj);
            }
            else if (type.IsString)
            {
                var output = ReadString(type, obj);
                Console.WriteLine($"  (length: {output.length})");
                Console.WriteLine($"  {output.text}");
            }
            else if (type.IsArray)
            {
                if (type.ComponentType.HasSimpleValue)
                {
                    ReadSimpleArray(type, obj);
                }
                else
                {
                    ReadComplexArray(type, obj);
                }
            }
            else if (type.IsEnum && type.GetEnumElementType() == ClrElementType.Int32)
            {
                ReadEnum(type, obj);
            }
        }

        static string GetOutput(ulong obj, ClrInstanceField field)
        {
            // If we don't have a simple value, return the address of the field in hex.
            if (!field.HasSimpleValue)
                return field.GetAddress(obj).ToString("X");

            object value = field.GetValue(obj);
            if (value == null)
                return "{error}";  // Memory corruption in the target process.

            // Decide how to format the string based on the underlying type of the field.
            switch (field.ElementType)
            {
                case ClrElementType.String:
                    // In this case, value is the actual string itself.
                    return (string)value;

                case ClrElementType.Array:
                case ClrElementType.SZArray:
                case ClrElementType.Object:
                case ClrElementType.Class:
                case ClrElementType.FunctionPointer:
                case ClrElementType.NativeInt:
                case ClrElementType.NativeUInt:
                    // These types are pointers.  Print as hex.
                    return string.Format("{0:X}", value);

                default:
                    // Everything else will look fine by simply calling ToString.
                    return value.ToString();
            }
        }

        static void ReadException(ClrType type, ulong obj)
        {
            ReadExceptionHResult(type, obj);

            var ex = type.Heap.GetExceptionObject(obj);
            Console.WriteLine(ex.Type.Name);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.HResult.ToString("X"));

            foreach (var frame in ex.StackTrace)
                Console.WriteLine(frame.ToString());
        }

        static void ReadExceptionHResult(ClrType type, ulong obj)
        {
            ClrInstanceField field = type.GetFieldByName("_HResult");
            Debug.Assert(field.ElementType == ClrElementType.Int32);

            int value = (int)field.GetValue(obj);
            Console.WriteLine("  Exception 0x{0:X} hresult: 0x{1:X}", obj, value);

            // get value detail
            var output = GetOutput(obj, field);
            Console.WriteLine("  " + output);
        }

        static (int length, string text) ReadString(ClrType type, ulong obj)
        {
            ClrInstanceField lengthField = type.GetFieldByName("_stringLength");
            if (lengthField == null)
                return (0, String.Empty);

            var stringLength = (int)lengthField.GetValue(obj);
            if (stringLength <= 0)
                return (stringLength, String.Empty);

            var text = (string)type.GetValue(obj);
            return (stringLength, text);
        }

        static void ReadSimpleArray(ClrType type, ulong obj)
        {
            int len = type.GetArrayLength(obj);

            Console.WriteLine("Object: {0:X}", obj);
            Console.WriteLine("Length: {0}", len);
            Console.WriteLine("Elements:");

            for (int i = 0; i < len; i++)
                Console.WriteLine("{0,3} - {1}", i, type.GetArrayElementValue(obj, i));
        }

        static void ReadComplexArray(ClrType type, ulong obj)
        {
            int len = type.GetArrayLength(obj);

            Console.WriteLine("Object: {0:X}", obj);
            Console.WriteLine("Type:   {0}", type.Name);
            Console.WriteLine("Length: {0}", len);
            for (int i = 0; i < len; i++)
            {
                ulong addr = type.GetArrayElementAddress(obj, i);
                foreach (var field in type.ComponentType.Fields)
                {
                    string output;
                    if (field.HasSimpleValue)
                    {
                        var tmpOutput = field.GetValue(addr, true);
                        if (tmpOutput == null)
                            continue;
                        output = tmpOutput.ToString();        // <- true here, as this is an embedded struct
                    }
                    else
                    {
                        output = field.GetAddress(addr, true).ToString("X");   // <- true here as well
                    }

                    Console.WriteLine("{0}  +{1,2:X2} {2} {3} = {4}", i, field.Offset, field.Type.Name, field.Name, output);
                }
            }
        }

        static void ReadEnum(ClrType type, ulong obj)
        {
            int objValue = (int)type.GetValue(obj);

            bool found = false;
            foreach (var name in type.GetEnumNames())
            {
                int value;
                if (type.TryGetEnumValue(name, out value) && objValue == value)
                {
                    Console.WriteLine("{0} - {1}", value, name);
                    found = true;
                    break;
                }
            }

            if (!found)
                Console.WriteLine("{0} - {1}", objValue, "Unknown");
        }

        static void ReadThreadStaticVariables(ClrRuntime runtime, ClrType type)
        {
            foreach (ClrAppDomain appDomain in runtime.AppDomains)
            {
                foreach (ClrThread thread in runtime.Threads)
                {
                    foreach (ClrThreadStaticField field in type.ThreadStaticFields)
                    {
                        if (field.HasSimpleValue)
                            Console.WriteLine("{0}.{1} ({2}, {3:X}) = {4}", type.Name, field.Name, appDomain.Id, thread.OSThreadId, field.GetValue(appDomain, thread));
                    }
                }
            }
        }
    }
}
