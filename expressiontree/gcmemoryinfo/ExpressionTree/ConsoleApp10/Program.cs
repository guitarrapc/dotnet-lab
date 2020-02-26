using ClassLibrary1;
using System;

namespace ConsoleApp10
{
    class Program
    {
        static void Main(string[] args)
        {
            var method = GCMemoryStats.CreateGetGCMemoryInfoDelegateExpression();
            var result = method.Invoke();
            var (highMemoryLoadThresholdBytes, memoryLoadBytes, totalAvailableMemoryBytes, heapSizeBytes, fragmentedBytes) = GCMemoryStats.GetGCMemoryInfoPropertieValues(typeof(GCMemoryInfo), result);
            Console.WriteLine(highMemoryLoadThresholdBytes);
            Console.WriteLine(memoryLoadBytes);
            Console.WriteLine(totalAvailableMemoryBytes);
            Console.WriteLine(heapSizeBytes);
            Console.WriteLine(fragmentedBytes);
        }

    }
}
