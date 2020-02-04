using Microsoft.Extensions.Configuration;
using StackExchange.Redis.Extensions.Utf8Json;
using System;
using System.Threading.Tasks;

namespace RedisConnectionConnectionStrings
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Create CacheClient and connecting ....");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            // tls
            //var redisConnectionString = config.GetConnectionString("redis-tls");
            // non-tls
            var redisConnectionString = config.GetConnectionString("redis");
            var redisConfig = new RedisConfig("redis", redisConnectionString);
            var connection = new RedisConnection(redisConfig, new Utf8JsonSerializer());

            var cache = new CacheClient(connection);

            // Commands
            await cache.ExecuteAsync("PING");
            await cache.StringGetAsync("Message");
            await cache.StringSetAsync("Message", "Hello! The cache is working from a .NET Core console app!");
            await cache.StringGetAsync("Message");
            await cache.ExecuteAsync("CLIENT", "LIST");

            // POCO
            var employee = new Employee
            {
                Id = "007",
                Name = "Davide Columnbo",
                Age = 100,
            };
            await cache.ObjectSetAsync<Employee>($"{employee.Id}_{employee.Name}", employee);
            var result = await cache.ObjectGetAsync<Employee>($"{employee.Id}_{employee.Name}");
            Console.WriteLine($"Id {result.Id}; Name {result.Name}; Age {result.Age}");
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
