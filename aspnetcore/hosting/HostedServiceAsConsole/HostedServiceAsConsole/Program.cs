using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HostedServiceAsConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateBuilder().RunConsoleAsync();
        }

        private static IHostBuilder CreateBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => services.AddHostedService<ConsoleHostingService>());
        }
    }

    public class ConsoleHostingService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<ConsoleHostingService> _logger;

        public ConsoleHostingService(IHostApplicationLifetime applicationLifetime, ILogger<ConsoleHostingService> logger)
        {
            _cts = new CancellationTokenSource();
            _applicationLifetime = applicationLifetime;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");
            var model = new LoopModel();
            model.StartAsync(_cts.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }

    public class LoopModel
    {
        public async Task StartAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), ct);
            }
        }
    }
}
