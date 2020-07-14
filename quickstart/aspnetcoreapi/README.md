## TL;DR

ASP.NET Core API quick start app.

[Dockerfile](https://github.com/guitarrapc/dotnet-lab/blob/master/quickstart/aspnetcoreapi/aspnetcore-api-quickstart/Dockerfile)

## Image Tags

* `latest`: latest version of tag.
* `3.1`: ASP.NET Core API 3.1

## Run

Run latest.

```shell
docker run --rm -p 8088:80 guitarrapc/asptnetcoreapi-quickstart:latest
```

Run ASP.NET Core 3.1.

```shell
docker run --rm -p 8088:80 guitarrapc/asptnetcoreapi-quickstart:3.1
```

cURL request.

```shell
curl http://localhost:8088/healthz
curl http://localhost:8088/api/hello
curl http://localhost:8088/api/hello?name=hoge
curl http://localhost:8088/api/weatherforecast
```

## Load Test

ab

```shell
$ ab -c 10 -n 10000 http://localhost:8088/weatherforecast
This is ApacheBench, Version 2.3 <$Revision: 1843412 $>
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Licensed to The Apache Software Foundation, http://www.apache.org/
Benchmarking localhost (be patient)
Completed 1000 requests
Completed 2000 requests
Completed 3000 requests
Completed 4000 requests
Completed 5000 requests
Completed 6000 requests
Completed 7000 requests
Completed 8000 requests
Completed 9000 requests
Completed 10000 requests
Finished 10000 requests
Server Software:        Kestrel
Server Hostname:        localhost
Server Port:            8088
Document Path:          /weatherforecast
Document Length:        0 bytes
Concurrency Level:      10
Time taken for tests:   8.022 seconds
Complete requests:      10000
Failed requests:        0
Non-2xx responses:      10000
Total transferred:      1180000 bytes
HTML transferred:       0 bytes
Requests per second:    1246.54 [#/sec] (mean)
Time per request:       8.022 [ms] (mean)
Time per request:       0.802 [ms] (mean, across all concurrent requests)
Transfer rate:          143.64 [Kbytes/sec] received
Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.5      0       1
Processing:     1    8   3.3      7      76
Waiting:        1    7   3.1      6      59
Total:          2    8   3.3      7      76
Percentage of the requests served within a certain time (ms)
  50%      7
  66%      8
  75%      9
  80%     10
  90%     11
  95%     13
  98%     16
  99%     19
 100%     76 (longest request)
```

bombardier

```shell
 $ bombardier http://localhost:8088/weatherforecast
Bombarding http://localhost:8088/weatherforecast for 10s using 125 connection(s)
[=================================================================================================================] 10s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec      9867.06    2945.39   16996.77
  Latency       12.67ms     2.46ms   122.99ms
  HTTP codes:
    1xx - 0, 2xx - 0, 3xx - 0, 4xx - 98678, 5xx - 0
    others - 0
  Throughput:     1.66MB/s
```