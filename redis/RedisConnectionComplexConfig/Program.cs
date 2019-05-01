using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Threading.Tasks;

namespace RedisConnectionComplexConfig
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cache = new CacheClient();

            // Commands
            await ExecuteAsync(cache.Db, "PING");
            await StringGetAsync(cache.Db, "Message");
            await StringSetAsync(cache.Db, "Message", "Hello! The cache is working from a .NET Core console app!");
            await StringGetAsync(cache.Db, "Message");
            await ExecuteAsync(cache.Db, "CLIENT", "LIST");

            // POCO
            var employee = new Employee
            {
                Id= "007",
                Name = "Davide Columnbo",
                Age = 100,
            };
            await ObjectSetAsync(cache.RedisDb, $"{employee.Id}_{employee.Name}", employee);
            await ObjectGetAsync(cache.RedisDb, $"{employee.Id}_{employee.Name}");

            cache.Dispose();
        }

        static async Task ExecuteAsync(IDatabase cache, string cacheCommand, params object[] args)
        {
            Console.WriteLine($"Cache command  : {cacheCommand}");
            Console.WriteLine("Cache response : " + await cache.ExecuteAsync(cacheCommand, args));
        }

        static async Task StringGetAsync(IDatabase cache, string key)
        {
            Console.WriteLine($"Cache command  : GetString {key}");
            Console.WriteLine("Cache response : " + await cache.StringGetWithExpiryAsync(key));
        }

        static async Task StringSetAsync(IDatabase cache, string key, string message)
        {
            Console.WriteLine($"Cache command  : SetString {key}");
            Console.WriteLine("Cache response : " + await cache.StringSetAsync(key, message, when: When.NotExists));
        }

        static async Task ObjectSetAsync(IRedisDatabase cache, string key, Employee employee)
        {
            Console.WriteLine($"Cache command  : Add {key}");
            Console.WriteLine("Cache response : " + await cache.AddAsync($"{employee.Id}_{employee.Name}", employee, when: When.NotExists));
        }

        static async Task ObjectGetAsync(IRedisDatabase cache, string key)
        {
            Console.WriteLine($"Cache command  : Get {key}");
            var employee = await cache.GetAsync<Employee>(key);
            Console.WriteLine("Cache response : " + employee.Name);
        }
    }
    public class Employee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        // DO NOT USE parameterized CONSTRUCTOR for deserialization
    }
}
