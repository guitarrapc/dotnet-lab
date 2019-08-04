# Single Executable assembly

from dotnet core 3.0 preview 5, you can create single executable assembly.

Here's feature list.

* publish single file or not
* Selfcontained or Framework dependent
* single file is trimed or not.

See details in source code and md.

> source: https://github.com/dotnet/sdk/blob/master/src/Tasks/Microsoft.NET.Build.Tasks/targets/Microsoft.NET.Publish.targets
> 
> readme: https://github.com/dotnet/designs/blob/master/accepted/single-file/design.md

## Framework dependent Build

specify `--self-contained=false` on build.

> Tips: Make sure specify runtime on build or csproj. see bottom tips.

You can build single file with.

```dotnet
dotnet publish -r win10-x64 /p:PublishSingleFile=true --self-contained=false
```

## Selfcontained Build

You can build single file with.

> Tips: Make sure specify runtime on build or csproj.

```dotnet
dotnet publish -r win10-x64 /p:PublishSingleFile=true
```

## csproj

Not only CLI, but csproj also can specify single file or not.

> TIPS: You will find prolems when mixing dotnet core 2.1 and 3.0 in csproj w/VisualStudio. see TIPS.

**Split build by condition**

Single Executable build may only required on CI build, not in VisualStudio or any other local build, let's add build condition for single-file.

With this sample csproj, VS will build as dotnetcore 2.1. a single-file is only generated when passing `/p:PublishSingleFile=true` to the dotnet build/publish.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.1</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <PropertyGroup Condition="'$(PublishSingleFile)' == 'true'">
    <TargetFramework>netcoreapp3.0</TargetFramework>
    <PublishSingleFile>true</PublishSingleFile>
    <PublishTrimmed>true</PublishTrimmed>
    <IncludeSymbolsInSingleFile>true</IncludeSymbolsInSingleFile>
  </PropertyGroup>

</Project>
```

normal dotnet publish will not generate single file and use dotnet core 2.1.

```shell
dotnet publish -c Release
```

dotnet core 3.0 will be used only when trying to generate single file.

```shell
dotnet publish -c Release -r win10-x64 /p:PublishSingleFile=true
```

**PublishSingleFile**

PublishSingleFile will let you build single-file on publish.

```xml
<PropertyGroup>
  <PublishSingleFile>true</PublishSingleFile>
</PropertyGroup>
```

**PublishTrimmed**

PublishTrimmed dramatically reduce your single file size.

```xml
<PropertyGroup>
  <PublishTrimmed>true</PublishTrimmed>
</PropertyGroup>
```

Not Trimmed | Trimmed
---- | ----
67462kb | 25925kb

when not trimed.

![image](https://user-images.githubusercontent.com/3856350/62417198-e8d93c80-b684-11e9-9631-4853150480be.png)

when trimmed.

![image](https://user-images.githubusercontent.com/3856350/62417201-f8f11c00-b684-11e9-8f8e-70fc482725bc.png)


**IncludeSymbolsInSingleFile**

enable to contains IL .pdb file, and the native .ni.pdb / app.guid.map into single file.

```xml
<PropertyGroup>
  <IncludeSymbolsInSingleFile>true</IncludeSymbolsInSingleFile>
</PropertyGroup>
```

## TIPS: Visual Studio issues

Make sure dotnet core 3.0 is still in preview, Visual Stduio will have limitation with preview SDK, and also mixing multiple sdk version.

If you build dotnet core 3.0 on cli and VS is using netcoreapp 2.1, you will find following error.

![image](https://user-images.githubusercontent.com/3856350/62417277-ba5c6100-b686-11e9-9111-6bece18a37ce.png)

```
Severity	Code	Description	Project	File	Line	Suppression State
Error	NETSDK1005	Assets file 'D:\git\guitarrapc\dotnet-lab\singleexecutable\SimpleSingleExecutable\SimpleSingleExecutable\obj\project.assets.json' doesn't have a target for '.NETCoreApp,Version=v2.1'. Ensure that restore has run and that you have included 'netcoreapp2.1' in the TargetFrameworks for your project.	SimpleSingleExecutable	C:\Program Files\dotnet\sdk\2.2.204\Sdks\Microsoft.NET.Sdk\targets\Microsoft.PackageDependencyResolution.targets	208	
```

It's because `obj/project.assets.json` was generated as netcore 3.0 and Visual Studio will not reload when it's re-generated.
So, once you seen this error do following.

* Close Visual Studio.
* Remove /obj and /bin folders.
* Reopen Visual Studio.

![image](https://user-images.githubusercontent.com/3856350/62417308-3f477a80-b687-11e9-8c88-973457422902.png)

## TIPS: Runtime IDentifier (RID) list

> https://docs.microsoft.com/en-US/dotnet/core/rid-catalog

Windows 

```
win-x64
win-x86
win-arm
win-arm64
```

Linux

```
linux-x64 (Most desktop distributions like CentOS, Debian, Fedora, Ubuntu and derivatives)
linux-musl-x64 (Lightweight distributions using musl like Alpine Linux)
linux-arm (Linux distributions running on ARM like Raspberry Pi)
```

macOS

```
osx-x64
```

## TIPS: NuGet Global Tool

Single Executable could not be NuGet Global Tool.

If you add following to handle nuget as a single executable, you will get error on publish, not on build.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.1</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <PropertyGroup>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <PackAsTool>true</PackAsTool>
    <PackageId>test</PackageId>
    <PackageVersion>$(Version)</PackageVersion>
    <Authors>guitarrapc</Authors>
    <Copyright>guitarrapc</Copyright>
    <Description>Test.</Description>
    <PackageProjectUrl>https://github.com/guitarrapc/dotnet-lab</PackageProjectUrl>
    <RepositoryUrl>$(PackageProjectUrl)</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
  </PropertyGroup>

  <PropertyGroup Condition="'$(PublishSingleFile)' == 'true'">
    <TargetFramework>netcoreapp3.0</TargetFramework>
    <PublishSingleFile>true</PublishSingleFile>
    <PublishTrimmed>true</PublishTrimmed>
    <IncludeSymbolsInSingleFile>true</IncludeSymbolsInSingleFile>
  </PropertyGroup>

</Project>

```

Error seems here, and show `ComputeManagedAssemblies` issue. 

```
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018: The "ComputeManagedAssemblies" task failed unexpectedly. [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018: System.IO.FileNotFoundException: Could not find file 'D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\obj\Release\netcoreapp3.0\win10-x64\GlobalToolSingleExecutable.exe'. [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018: File name: 'D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\obj\Release\netcoreapp3.0\win10-x64\GlobalToolSingleExecutable.exe' [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at System.IO.FileStream.ValidateFileHandle(SafeFileHandle fileHandle) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at System.IO.FileStream.CreateFileOpenHandle(FileMode mode, FileShare share, FileOptions options) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share, Int32 bufferSize, FileOptions options) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access, FileShare share) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at Mono.Cecil.ModuleDefinition.GetFileStream(String fileName, FileMode mode, FileAccess access, FileShare share) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at Mono.Cecil.ModuleDefinition.ReadModule(String fileName, ReaderParameters parameters) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at Mono.Cecil.ModuleDefinition.ReadModule(String fileName) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at Utils.IsManagedAssembly(String fileName) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at ILLink.Tasks.ComputeManagedAssemblies.<>c.<Execute>b__8_0(ITaskItem f) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at System.Linq.Enumerable.WhereArrayIterator`1.ToArray() [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at System.Linq.Enumerable.ToArray[TSource](IEnumerable`1 source) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at ILLink.Tasks.ComputeManagedAssemblies.Execute() [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at Microsoft.Build.BackEnd.TaskExecutionHost.Microsoft.Build.BackEnd.ITaskExecutionHost.Execute() [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
C:\Program Files\dotnet\sdk\3.0.100-preview7-012821\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.ILLink.targets(142,5): error MSB4018:    at Microsoft.Build.BackEnd.TaskBuilder.ExecuteInstantiatedTask(ITaskExecutionHost taskExecutionHost, TaskLoggingContext taskLoggingContext, TaskHost taskHost, ItemBucket bucket, TaskExecutionMode howToExecuteTask) [D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj]
```

It's natural that NuGet Package publish failed because of runtime, it should be a Framework dependence.

Solution is add condition to nuget properties to be build when it is not an single file.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.1</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <PropertyGroup Condition="'$(PublishSingleFile)' != 'true'">
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <PackAsTool>true</PackAsTool>
    <PackageId>test</PackageId>
    <PackageVersion>$(Version)</PackageVersion>
    <Authors>guitarrapc</Authors>
    <Copyright>guitarrapc</Copyright>
    <Description>Test.</Description>
    <PackageProjectUrl>https://github.com/guitarrapc/dotnet-lab</PackageProjectUrl>
    <RepositoryUrl>$(PackageProjectUrl)</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
  </PropertyGroup>

  <PropertyGroup Condition="'$(PublishSingleFile)' == 'true'">
    <TargetFramework>netcoreapp3.0</TargetFramework>
    <PublishSingleFile>true</PublishSingleFile>
    <PublishTrimmed>true</PublishTrimmed>
    <IncludeSymbolsInSingleFile>true</IncludeSymbolsInSingleFile>
  </PropertyGroup>

</Project>
```

You can publish for both nuget and global tool.

```shell
$ dotnet publish
Microsoft (R) Build Engine version 16.3.0-preview-19329-01+d31fdbf01 for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Restore completed in 22.88 ms for D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable.csproj.
  GlobalToolSingleExecutable -> D:\git\guitarrapc\dotnet-lab\singleexecutable\GlobalToolSingleExecutable\GlobalToolSingleExecutable\bin\Debug\netcoreapp2.1\publish\
```

Make sure if you build one of nuget or global-tool, you need to remove /bin and `dotnet restore` and `dotnet build` to publish other.

```
# build as single executable
dotnet restore
dotnet publish -c Release -r win10-x64 /p:PublishSingleFile=true

# remove bin/
rm -rf ./bin/

# build as nuget
dotnet restore
dotnet build
dotnet publish
```