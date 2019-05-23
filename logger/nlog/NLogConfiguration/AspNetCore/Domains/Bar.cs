using NLog; // not Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Domains
{
    public class Bar
    {
        // direct generate logger
        private static readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Bar()
        {
            // no DI
        }

        public void Piyo()
        {
            logger.Log(LogLevel.Info, "hogemogefugafuga");
        }
    }
}
