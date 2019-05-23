using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace AspNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((builder, logging) =>
                {
                    // disable existing defualt loggers
                    logging.ClearProviders();

                    // set loglevel with Logging section
                    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

                    // inject configuration for NLog.
                    // ref: https://github.com/NLog/NLog/wiki/ConfigSetting-Layout-Renderer
                    NLog.Extensions.Logging.ConfigSettingLayoutRenderer.DefaultConfiguration = builder.Configuration;

                    // load config
                    NLog.LogManager.LoadConfiguration($"nlog.config");
                    // if overload additional configuration
                    //NLog.LogManager.LoadConfiguration($"nlog.{builder.HostingEnvironment.EnvironmentName}.config");
                })
                .UseNLog()
                .UseStartup<Startup>();
    }
}
