using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace My.Logging.Infrastructure
{
    // marker for application logger
    public interface IApplicationLogger
    {
    }
    // marker for singleton register
    public interface ISingletonApplicationLogger : IApplicationLogger
    {
    }
}
