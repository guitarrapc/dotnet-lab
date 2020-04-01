## HostedService

## TERM signal

TERM signal will be sent in various situations.

1. kill
1. docker stop
1. Kubernetes pod stop, eviction

If your app is running as `PID 1`, easily happen when running with docker, k8s and other containers, you must explictly handle TERM signal.
Let's check summary and example apps.

## Terminate .NET Core app when TERM signal

If you just want to quit app when TERM signal invoked, just use `GenericHost` and host your app.
Console or original hosted app will be...

1. use `GenericHost` and `RunConsoleAsync`.
1. use `GenericHost`, `UseConsoleLifetime()` and `RunAsync()`

```csharp
// 1
await Host.CreateDefaultBuilder()
    .RunConsoleAsync();
// 2
await Host.CreateDefaultBuilder()
    .UseConsoleLifeTime()
    .Build()
    .RunAsync();
```

If app is ASP.NET Core, then TERM signal will be handle without `UseConsoleLifetime` or `RunConsoleAsync`.
So that original generated asp.net core code is fine, off course you can use `RunConsoleAsync` if any reason.

1. use `GenericHost`, `ConfigureWebHostDefaults` and `RunAsync`.
1. use `GenericHost`, `ConfigureWebHostDefaults` and `RunConsoleAsync`.

```csharp
// 1
Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .Build()
    .RunAsync();

// 2
Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .RunConsoleAaync();
```

## IHostedService to treat Graceful shutdown TERM signal.

.NET Core 2.2 and above can handle TERM signal with `IHostedService`.
If you don't have strong reason, you should use `IHostedService.StopAsync` to handle graceful shutdown.

`IHostedService` can be use with both .NET Core console and ASP.NET Core.

It means, if you want handle any operation when TERM signal invoked and before shutdown, you must write your clean up code on `IHostdSErvice.StopAsync`.

```csharp
// Program.cs
await Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) => services.AddHostedService<MyHostingService>())
    .RunConsoleAsync();

// MyHostingService
public class MyHostingService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // any invokation when HostedService starts. don't await long running start operation, just FireAndForget it.
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // any invokation when TERM signal invoked and before Shutdown.
        // You must complete all operation within 5 sec in default. (can be extend)
        return Task.CompletedTask;
    }
}
```

## Extend Shutdown Timeout

Default Shutdown timeout, it means timeout since invoke StopAsync and until exit StopAsync, is 5 second.
You must complete your StopAsync operation within shutdown timeout, 5 second.
To extend this Shutdown Duration, use `IApplicationLifetime.ShutdownTimeout`.
Here's example extend to 10 second.

```csharp
await Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        // default 5sec. extend to 10 sec for Graceful shutdown
        services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(10));
    })
    .ConfigureServices((context, services) => services.AddHostedService<MyHostingService>())
    .RunConsoleAsync();
```