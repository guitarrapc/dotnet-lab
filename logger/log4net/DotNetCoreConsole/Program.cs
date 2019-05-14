using log4net;
using log4net.Config;
using log4net.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;

[assembly: log4net.Config.XmlConfigurator()]
namespace log4net_config_sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var servicesProvider = Build();
            var runner = servicesProvider.GetRequiredService<Runner>();

            runner.DoAction("Action1");

            Console.WriteLine("End logging");

            var logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            logger.Info("hogemoge");


            Console.ReadLine();

            LogManager.Flush(1000);
            LogManager.Shutdown();
        }

        private static ServiceProvider Build()
        {
            return new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddLog4Net(new Log4NetProviderOptions
                    {
                        Log4NetConfigFileName = "log4net.config",
                        Watch = true,
                    });
                })
                .AddTransient<Runner>()
                .BuildServiceProvider();
        }
    }
}