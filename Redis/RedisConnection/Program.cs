using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RedisConnection
{
    class Program
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            // simple
            var cacheConnection = "localhost:6379";
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection => lazyConnection.Value;

        static async Task Main(string[] args)
        {
            IDatabase cache = lazyConnection.Value.GetDatabase();

            // Commands
            await ExecuteAsync(cache, "PING");
            await StringGetAsync(cache, "Message");
            await StringSetAsync(cache, "Message", "Hello! The cache is working from a .NET Core console app!");
            await StringGetAsync(cache, "Message");
            await ExecuteAsync(cache, "CLIENT", "LIST");

            lazyConnection.Value.Dispose();
        }

        static async Task ExecuteAsync(IDatabase cache, string cacheCommand, params object[] args)
        {
            Console.WriteLine($"Cache command  : {cacheCommand}");
            Console.WriteLine("Cache response : " + await cache.ExecuteAsync(cacheCommand, args));
        }

        static async Task StringGetAsync(IDatabase cache, string key)
        {
            Console.WriteLine($"Cache command  : GetString {key}");
            Console.WriteLine("Cache response : " + await cache.StringGetAsync(key));
        }

        static async Task StringSetAsync(IDatabase cache, string key, string message)
        {
            Console.WriteLine($"Cache command  : SetString {key}");
            Console.WriteLine("Cache response : " + await cache.StringSetAsync(key, message));
        }
    }
}
