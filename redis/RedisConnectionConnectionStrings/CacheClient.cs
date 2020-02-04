using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RedisConnectionConnectionStrings
{
    public class RedisConfig
    {
        public string Name { get; }
        public ConfigurationOptions Options { get; }
        public int? Database { get; }

        public RedisConfig(string name, string connectionStrings, int database = default)
        {
            Name = name;
            Options = ConfigurationOptions.Parse(connectionStrings, true);
            Database = database;
            if (Options.Ssl)
            {
                Options.CertificateValidation += ConfigurationOptions_CertificateValidation;
                Options.CertificateSelection += ConfigurationOptions_CertificateSelection;
            }
        }

        private bool ConfigurationOptions_CertificateValidation(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            => true;
        private System.Security.Cryptography.X509Certificates.X509Certificate ConfigurationOptions_CertificateSelection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
            => new X509Certificate2("cert.crt");
    }

    public class RedisConnection
    {
        public RedisConfig Config { get; }
        public IDatabaseAsync Database => Config.Database.HasValue
            ? GetConnection().GetDatabase(Config.Database.Value)
            : GetConnection().GetDatabase();
        public ITransaction Transaction => ((IDatabase)Database).CreateTransaction();
        public IServer[] Servers => Config.Options.EndPoints.Select(this.GetConnection(), (x, c) => c.GetServer(x)).ToArray();
        public ISerializer Serializer{ get; }

        public RedisConnection(RedisConfig config, ISerializer serializer)
        {
            Config = config;
            Serializer = serializer;
        }

        private readonly object gate = new object();
        private ConnectionMultiplexer connection = null;
        public ConnectionMultiplexer GetConnection()
        {
            lock (this.gate)
            {
                if (this.connection == null || !this.connection.IsConnected)
                {
                    try
                    {
                        this.connection = ConnectionMultiplexer.Connect(this.Config.Options);
                        this.connection.ConfigurationChanged += (_, e) => { Console.WriteLine($"ConfigurationChanged: endpoint {e.EndPoint}"); };
                        this.connection.ConfigurationChangedBroadcast += (_, e) => { Console.WriteLine($"ConfigurationChangedBroadcast: endpoint {e.EndPoint}"); };
                        this.connection.ConnectionFailed += (_, e) => { Console.WriteLine($"ConnectionFailed: endpoint {e.EndPoint}"); };
                        this.connection.ConnectionRestored += (_, e) => { Console.WriteLine($"ConnectionRestored: endpoint {e.EndPoint}"); };
                        this.connection.ErrorMessage += (_, e) => { Console.WriteLine($"ErrorMessage: endpoint {e.EndPoint}"); };
                        this.connection.HashSlotMoved += (_, e) => { Console.WriteLine($"HashSlotMoved: oldEndpoint {e.OldEndPoint}; newEndpoint {e.NewEndPoint}"); };
                        this.connection.InternalError += (_, e) => { Console.WriteLine($"InternalError: endpoint {e.EndPoint}"); };
                    }
                    catch
                    {
                        this.connection = null;
                        throw;
                    }
                }
                return this.connection;
            }
        }
    }

    public class CacheClient
    {
        public RedisConnection Connection { get; }

        public CacheClient(RedisConnection connection)
        {
            Connection = connection;
        }

        public async Task ExecuteAsync(string cacheCommand, params object[] args)
        {
            Console.WriteLine($"Cache command  : {cacheCommand}");
            Console.WriteLine("Cache response : " + await Connection.Database.ExecuteAsync(cacheCommand, args));
        }

        public async Task StringGetAsync(string key)
        {
            Console.WriteLine($"Cache command  : GetString {key}");
            Console.WriteLine("Cache response : " + await Connection.Database.StringGetWithExpiryAsync(key));
        }

        public async Task StringSetAsync(string key, string message)
        {
            Console.WriteLine($"Cache command  : SetString {key}");
            Console.WriteLine("Cache response : " + await Connection.Database.StringSetAsync(key, message, when: When.NotExists));
        }

        public async Task ObjectSetAsync<T>(string key, T obj)
        {
            Console.WriteLine($"Cache command  : Add {key}");
            var value = await Connection.Serializer.SerializeAsync(obj);
            Console.WriteLine("Cache response : " + await Connection.Database.StringSetAsync(key, value, when: When.NotExists));
        }

        public async Task<T> ObjectGetAsync<T>(string key) where T : class
        {
            Console.WriteLine($"Cache command  : Get {key}");
            var value = await Connection.Database.StringGetAsync(key);
            var obj = await Connection.Serializer.DeserializeAsync<T>(value);
            return obj;
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<TResult> Select<T, TState, TResult>(this IEnumerable<T> source, TState state, Func<T, TState, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            foreach (var x in source)
                yield return selector(x, state);
        }
    }
}
