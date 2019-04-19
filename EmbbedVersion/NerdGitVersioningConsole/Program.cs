using System;

namespace NerdGitVersioningConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{nameof(ThisAssembly.AssemblyConfiguration)}: {ThisAssembly.AssemblyConfiguration}");
            Console.WriteLine($"{nameof(ThisAssembly.AssemblyFileVersion)}: {ThisAssembly.AssemblyFileVersion}");
            Console.WriteLine($"{nameof(ThisAssembly.AssemblyInformationalVersion)}: {ThisAssembly.AssemblyInformationalVersion}");
            Console.WriteLine($"{nameof(ThisAssembly.AssemblyName)}: {ThisAssembly.AssemblyName}");
            Console.WriteLine($"{nameof(ThisAssembly.AssemblyTitle)}: {ThisAssembly.AssemblyTitle}");
            Console.WriteLine($"{nameof(ThisAssembly.AssemblyVersion)}: {ThisAssembly.AssemblyVersion}");
            Console.WriteLine($"{nameof(ThisAssembly.GitCommitId)}: {ThisAssembly.GitCommitId}");
            Console.WriteLine($"{nameof(ThisAssembly.RootNamespace)}: {ThisAssembly.RootNamespace}");
        }
    }
}
