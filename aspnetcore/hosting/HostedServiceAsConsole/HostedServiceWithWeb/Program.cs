using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HostedServiceWithWeb
{
    // #Logging timeline.
    //info: Microsoft.Hosting.Lifetime[0]
    //      Now listening on: https://localhost:5001
    //info: Microsoft.Hosting.Lifetime[0]
    //      Now listening on: http://localhost:5000
    //info: Microsoft.Hosting.Lifetime[0]
    //      Application started.Press Ctrl+C to shut down.
    //info: Microsoft.Hosting.Lifetime[0]
    //     Hosting environment: Development
    //info: Microsoft.Hosting.Lifetime[0]
    //     Content root path: D:\git\guitarrapc\dotnet-lab\aspnetcore\hosting\HostedServiceAsConsole\HostedServiceWebApplication
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //     StartAsync
    //info: HostedServiceWebApplication.LoopModel[0]
    //     loop...
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //     ApplicationStarted
    //info: Microsoft.Hosting.Lifetime[0]
    //     Application started.Press Ctrl+C to shut down.
    //info: Microsoft.Hosting.Lifetime[0]
    //     Hosting environment: Production
    //info: Microsoft.Hosting.Lifetime[0]
    //     Content root path: D:\git\guitarrapc\dotnet-lab\aspnetcore\hosting\HostedServiceAsConsole\HostedServiceWebApplication
    //info: HostedServiceWebApplication.LoopModel[0]
    //     loop...
    //info: HostedServiceWebApplication.LoopModel[0]
    //     loop...
    //info: HostedServiceWebApplication.LoopModel[0]
    //     loop...
    //info: HostedServiceWebApplication.LoopModel[0]
    //     loop...
    //info: Microsoft.Hosting.Lifetime[0]
    //     Application is shutting down...
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //     ApplicationStopping
    //info: Microsoft.Hosting.Lifetime[0]
    //     Application is shutting down...
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //     StopAsync
    //info: HostedServiceWebApplication.LoopModel[0]
    //     cancel requested in model
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //      1
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //      2
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //      3
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //      4
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //      5
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //     StopAsync.DelayCompleted
    //info: HostedServiceWebApplication.ConsoleHostingService[0]
    //     ApplicationStopped
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // single startup with hostedservice
            await CreateHostBuilder(args)
                .ConfigureServices((context, services) => services.AddHostedService<ConsoleHostingService>())
                .ConfigureServices((context, services) => services.AddSingleton<LoopModel>())
                .ConfigureServices((context, services) =>
                {
                    // default 5sec. extend to 10 sec for Graceful shutdown
                    services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(10));
                })
                .RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class ConsoleHostingService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<ConsoleHostingService> _logger;
        private readonly LoopModel _model;

        public ConsoleHostingService(LoopModel model, IHostApplicationLifetime applicationLifetime, ILogger<ConsoleHostingService> logger)
        {
            _cts = new CancellationTokenSource();
            _applicationLifetime = applicationLifetime;
            _logger = logger;
            _model = model;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync"); // 1 (start)
            _applicationLifetime.ApplicationStarted.Register(() => _logger.LogInformation("ApplicationStarted")); // 3 (start) <- after StartAsync

            _applicationLifetime.ApplicationStopping.Register(() => _logger.LogInformation("ApplicationStopping")); // 1 (stop) <- Before StopAsync
            _applicationLifetime.ApplicationStopped.Register(() => _logger.LogInformation("ApplicationStopped")); // 6 (stop) <- after StopAsync
            _model.StartAsync(_cts.Token).ConfigureAwait(false);

            // must return before stop. otherwise hosting service will not stop even after Ctrl+C.
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync"); // 2 (stop)

            var i = 1;
            var timer = new Timer(_ =>
            {
                _logger.LogInformation(i.ToString()); // 4 (stop)
                    i++;
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            // cancel invoke.
            _cts.Cancel();

            // do work before Shutdown
            await Task.Delay(TimeSpan.FromSeconds(5)); // ShutdownTimeout must be longer than this delay time.
            _logger.LogInformation("StopAsync.DelayCompleted"); // 5 (stop)

            // shutdown application
            _applicationLifetime.StopApplication();
        }

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }

    public class LoopModel
    {
        private readonly ILogger<LoopModel> _logger;
        public LoopModel(ILogger<LoopModel> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                _logger.LogInformation("loop..."); // 2 (start)
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            // after cancellation
            _logger.LogInformation("cancel requested in model"); // 3 (stop)
        }
    }
}
