using MicroBatchFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DockerComposeDnsResolution
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await BatchHost.CreateDefaultBuilder()
                .ConfigureServices((context, services) => services.AddHttpClient())
                .RunBatchEngineAsync<DnsResolution>(args);
        }
    }

    public class DnsResolution : BatchBase
    {
        private readonly string hostAddress;
        private readonly string url;
        private readonly IHttpClientFactory httpclientFactory;

        public DnsResolution(IConfiguration configuration, IHttpClientFactory httpclientFactory)
        {
            hostAddress = configuration.GetValue<string>("HOSTADDRESS");
            url = configuration.GetValue<string>("URL");
            this.httpclientFactory = httpclientFactory;
        }

        /// <summary>
        /// dns resolution will only work on host, but not for url.
        /// </summary>
        /// <returns></returns>
        [Command("lookup")]
        public async Task Lookup()
        {
            this.Context.Logger.LogInformation($"hostAddress: {hostAddress}");
            this.Context.Logger.LogInformation($"url: {url}");

            var resHost = await Dns.GetHostAddressesAsync(hostAddress);
            var host = resHost?.FirstOrDefault();
            this.Context.Logger.LogInformation($"resolution count: {resHost?.Length ?? 0}, {nameof(hostAddress)}: {host}");

            //// MEMO: SHOULD NOT PASS URL. only host is available
            //// System.Net.Internals.SocketExceptionFactory+ExtendedSocketException (00000005, 6): No such device or address
            //var resUrl = await Dns.GetHostAddressesAsync(url);
            //this.Context.Logger.LogInformation($"resolution {nameof(url)}: {resUrl}");

            var http = httpclientFactory.CreateClient();
            var urlBase = $"http://{host}";

            while (!(await ForceStatusCheck(http, $"{urlBase}/health")))
                await Task.Delay(TimeSpan.FromSeconds(1));

            while (true)
            {
                var res = await http.GetStringAsync($"{urlBase}/api/values");
                this.Context.Logger.LogInformation(res);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private async Task<bool> ForceStatusCheck(HttpClient http, string url)
        {
            try
            {
                var res = await http.GetAsync(url);
                return res.IsSuccessStatusCode;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                this.Context.Logger.LogWarning($"{url} not responding. {ex.Message}");
                return false;
            }
        }
    }
}
