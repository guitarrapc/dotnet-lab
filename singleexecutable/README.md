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

You can specify single file or not in csproj.

> TIPS: You will find prolems when mixing dotnet core 2.1 and 3.0 in csproj w/VisualStudio. see TIPS.

**Split build by condition**

Single Executable build will only required on CI build, not in VisualStudio or any other local build.

So let's add condition for single-file.

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

But only when trying to run as single-file, dotnet core 3.0 will be used and single file are generated.

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

### TIPS: Visual Studio issues

Make sure dotnet core 3.0 is still in preview, so Visual Stduio will have limitation with preview SDK, and also mixin multiple sdk version.

If you build with dotnet core 3.0, and VS is using netcoreapp 2.1, you will find following error.

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
