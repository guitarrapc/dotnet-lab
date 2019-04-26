using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Utf8Json;
using System;

namespace RedisConnectionComplexConfig
{
    public class CacheClient : IDisposable
    {
        private static readonly Lazy<RedisConfiguration> lazyConfiguration = new Lazy<RedisConfiguration>(() =>
        {
            var config = new RedisConfiguration()
            {
                AbortOnConnectFail = true,
                KeyPrefix = "_my_key_prefix_",
                Hosts = new RedisHost[]
                {
                    new RedisHost(){Host = "localhost", Port = 6379},
                },
                AllowAdmin = true,
                ConnectTimeout = 3000,
                Database = 0,
                //Ssl = true,
                //Password = "my_super_secret_password",
                ServerEnumerationStrategy = new ServerEnumerationStrategy()
                {
                    Mode = ServerEnumerationStrategy.ModeOptions.All,
                    TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                    UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
                }
            };
            return config;
        });
        private static readonly Lazy<RedisCacheConnectionPoolManager> lazyConnectionPool = new Lazy<RedisCacheConnectionPoolManager>(() => new RedisCacheConnectionPoolManager(lazyConfiguration.Value));
        private static readonly Lazy<RedisCacheClient> lazyClient = new Lazy<RedisCacheClient>(() => new RedisCacheClient(lazyConnectionPool.Value, new Utf8JsonSerializer(), lazyConfiguration.Value));
        private static readonly Lazy<IRedisDatabase> lazyRedisDatabase = new Lazy<IRedisDatabase>(() => lazyClient.Value.GetDbFromConfiguration());
        private static readonly Lazy<IDatabase> lazyDatabase = new Lazy<IDatabase>(() => lazyRedisDatabase.Value.Database);

        public RedisCacheConnectionPoolManager ConnectionPool => lazyConnectionPool.Value;
        public RedisCacheClient Client => lazyClient.Value;
        public IRedisDatabase RedisDb => lazyRedisDatabase.Value;
        public IDatabase Db => lazyDatabase.Value;

        public void Dispose()
        {
            Db.Multiplexer.GetSubscriber().UnsubscribeAll();
            ConnectionPool.Dispose();
        }
    }
}
