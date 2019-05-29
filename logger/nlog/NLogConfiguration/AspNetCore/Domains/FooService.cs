using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Domains
{
    public interface IFooService
    {
        void Foo();
    }

    public class FooService : IFooService
    {
        private readonly ILogger<FooService> logger;
        public FooService(ILogger<FooService> logger)
        {
            this.logger = logger;
        }

        public void Foo()
        {
            logger.LogInformation("Foo");
        }
    }
}
