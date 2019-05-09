## Redis

launch redis on docker and connect to it.

### RedisConnection

Simple Redis connection app with [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis).

### RedisConnectionComplexConfig

[StackExchange.Redis.Extensions](https://github.com/imperugo/StackExchange.Redis.Extensions) offers much rich taste of Configuration and connections.
This sample define connection with code.
Also using [Utf8json](https://github.com/neuecc/Utf8Json) to serialize/deserialize object with redis.

### RedisConnectionAppSettings

Similar to RedisConnectionComplexConfig but using appsettings.json to define connections.

### RedisConnectionElastiCacheEncrypted

Similar to RedisConnectionAppSettings, but connect to Elasticache which enables [In-Transit Encryption (TLS)](https://docs.aws.amazon.com/AmazonElastiCache/latest/red-ug/in-transit-encryption.html).

You need enable `ssl:true` in appsettings.json.

### RedisConnectionTls

Similar to RedisConnectionAppSettings, but ssl(tls) connect to stunnel which proxy redis.

> [Stunnel \- Securing Your Redis Traffic In SSL \| Redis Labs](https://redislabs.com/blog/stunnel-secure-redis-ssl/)

Run following shell before run application.

```shell
openssl genrsa -out certs/key.pem 4096
openssl req -new -x509 -key certs/key.pem -out certs/cert.pem -days 1826
openssl x509 -outform der -in certs/cert.pem -out RedisConnectionTls/cert.crt
```

Make sure you have input public ip or localhost to `Organization Name` and `Common Name` when generating cert.pem.

> [c\# \- Using StackExchange\.Redis and stunnel to create an SSL connection on Redis \- Stack Overflow](https://stackoverflow.com/questions/46022944/using-stackexchange-redis-and-stunnel-to-create-an-ssl-connection-on-redis)