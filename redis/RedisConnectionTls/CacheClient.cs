using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Utf8Json;
using System;
using System.Security.Cryptography.X509Certificates;

namespace RedisConnectionTls
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
            config.ConfigurationOptions.CertificateSelection += ConfigurationOptions_CertificateSelection;
            // MEMO: Don't do this in production (Dev only)
            // hack for local redis tls docker
            config.ConfigurationOptions.CertificateValidation += ConfigurationOptions_CertificateValidation;
            ConnectionPool = new RedisCacheConnectionPoolManager(this.config);
            Client = new RedisCacheClient(ConnectionPool, new Utf8JsonSerializer(), config);
            RedisDb = Client.GetDbFromConfiguration();
            Db = RedisDb.Database;
        }

        private bool ConfigurationOptions_CertificateValidation(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            => true;

        private System.Security.Cryptography.X509Certificates.X509Certificate ConfigurationOptions_CertificateSelection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
            => new X509Certificate2("cert.crt");

        public void Dispose()
        {
            Db.Multiplexer.GetSubscriber().UnsubscribeAll();
            ConnectionPool.Dispose();
        }
    }
}
