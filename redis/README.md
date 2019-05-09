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
