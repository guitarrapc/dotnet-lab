using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace log4net_config_sample
{
    public class Runner
    {
        private readonly ILogger<Runner> logger;

        public Runner(ILogger<Runner> logger)
        {
            this.logger = logger;
        }

        public void DoAction(string name)
        {
            logger.LogDebug(20, "Doing hard work! {Action}", name);

            logger.LogDebug("debug");
            logger.LogInformation("info");
            logger.LogWarning("warn");
            logger.LogError("error");
            logger.LogCritical("critical");

            foreach (var item in Enumerable.Range(1, 10000))
            {
                logger.LogInformation($"{item}");
            }
        }


    }
}
