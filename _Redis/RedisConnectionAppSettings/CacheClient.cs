using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Utf8Json;
using System;

namespace RedisConnectionAppSettings
{
    public class CacheClient : IDisposable
    {
        public RedisCacheConnectionPoolManager ConnectionPool { get; }
        public RedisCacheClient Client { get; }
        public IRedisDatabase RedisDb { get; }
        public IDatabase Db { get; }

        private RedisConfiguration config;

        public CacheClient(RedisConfiguration config)
        {
            this.config = config;
            ConnectionPool = new RedisCacheConnectionPoolManager(this.config);
            Client = new RedisCacheClient(ConnectionPool, new Utf8JsonSerializer(), config);
            RedisDb = Client.GetDbFromConfiguration();
            Db = RedisDb.Database;

        }
        public void Dispose()
        {
            Db.Multiplexer.GetSubscriber().UnsubscribeAll();
            ConnectionPool.Dispose();
        }
    }
}
