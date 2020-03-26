using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HostedServiceWebApplication
{
    // Logging timeline.
    //StartAsync
    //loop...
    //ApplicationStarted
    //Application started. Press Ctrl+C to shut down.
    //Hosting environment: Production
    //Content root path: xxxxx\HostedServiceAsConsole\HostedServiceWebApplication
    //loop...
    //loop...
    //loop...
    //Application is shutting down...
    //ApplicationStopping
    //Application is shutting down...
    //StopAsync
    //ApplicationStopped
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Task.WhenAll(
                CreateHostBuilder(args)
                    .Build()
                    .RunAsync(),
                CreateBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(15));
                    })
                    .RunConsoleAsync()
            );
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static IHostBuilder CreateBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => services.AddHostedService<ConsoleHostingService>())
                .ConfigureServices((context, services) => services.AddSingleton<LoopModel>());
        }
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
            _applicationLifetime.ApplicationStarted.Register(() => _logger.LogInformation("ApplicationStarted")); // 2 (start)

            _applicationLifetime.ApplicationStopping.Register(() => _logger.LogInformation("ApplicationStopping")); // 1 (stop)
            _applicationLifetime.ApplicationStopped.Register(() => _logger.LogInformation("ApplicationStopped")); // 4 (stop)
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
                _logger.LogInformation(i.ToString());
                i++;
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            _cts.Cancel();
            await Task.Delay(TimeSpan.FromSeconds(10));

            _logger.LogInformation("StopAsync.DelayCompleted"); // 3 (stop)
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
                _logger.LogInformation("loop...");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            _logger.LogInformation("cancel requested in model");
        }
    }
}
