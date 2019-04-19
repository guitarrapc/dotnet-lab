using System;
using System.Diagnostics;
using System.Reflection;

[assembly: AssemblyVersion(ThisAssembly.Git.BaseVersion.Major + "." + ThisAssembly.Git.BaseVersion.Minor + "." + ThisAssembly.Git.BaseVersion.Patch)]
[assembly: AssemblyFileVersion(ThisAssembly.Git.SemVer.Major + "." + ThisAssembly.Git.SemVer.Minor + "." + ThisAssembly.Git.SemVer.Patch)]
[assembly: AssemblyInformationalVersion(ThisAssembly.Git.SemVer.Major + "." + ThisAssembly.Git.SemVer.Minor + "." + ThisAssembly.Git.BaseVersion.Patch + "+" + ThisAssembly.Git.Commit)]
namespace GitInfoConsoleCustom
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            var productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            Console.WriteLine($"{nameof(ThisAssembly.Git.Branch)}: {ThisAssembly.Git.Branch}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.BaseTag)}: {ThisAssembly.Git.BaseTag}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.Commit)}: {ThisAssembly.Git.Commit}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.Commits)}: {ThisAssembly.Git.Commits}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.IsDirty)}: {ThisAssembly.Git.IsDirty}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.Sha)}: {ThisAssembly.Git.Sha}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.Tag)}: {ThisAssembly.Git.Tag}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.BaseVersion.Major)}: {ThisAssembly.Git.BaseVersion.Major}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.BaseVersion.Minor)}: {ThisAssembly.Git.BaseVersion.Minor}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.BaseVersion.Patch)}: {ThisAssembly.Git.BaseVersion.Patch}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.SemVer.DashLabel)}: {ThisAssembly.Git.SemVer.DashLabel}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.SemVer.Label)}: {ThisAssembly.Git.SemVer.Label}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.SemVer.Major)}: {ThisAssembly.Git.SemVer.Major}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.SemVer.Minor)}: {ThisAssembly.Git.SemVer.Minor}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.SemVer.Patch)}: {ThisAssembly.Git.SemVer.Patch}");
            Console.WriteLine($"{nameof(ThisAssembly.Git.SemVer.Source)}: {ThisAssembly.Git.SemVer.Source}");
            Console.WriteLine($"{nameof(assemblyVersion)}: {assemblyVersion}");
            Console.WriteLine($"{nameof(fileVersion)}: {fileVersion}");
            Console.WriteLine($"{nameof(productVersion)}: {productVersion}");
        }
    }
}
