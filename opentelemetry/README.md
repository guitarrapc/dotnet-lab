## TL;DR

## Telemetry looks

### metrics

Prometheus metrics sample.

Access to http://127.0.0.1:9181/metrics/.

```
# HELP sample_measure_initializeSamplesample/measure/initialize
# TYPE sample_measure_initialize counter
sample_measure_initialize{dim1="value1"} 900 1589954404624
```

Following is sample prometheus.yml config. Adjust port,interval as needed.

```yaml
scrape_configs:
    # The job name is added as a label `job=<job_name>` to any timeseries scraped from this config.
    - job_name: 'OpenTelemetrySample'
    # metrics_path defaults to '/metrics'
    # scheme defaults to 'http'.
    static_configs:
    - targets: ['localhost:9184']
```

### tracer

zipkin sample.

Access to http://localhost:9411/zipkin/.

![image](https://user-images.githubusercontent.com/3856350/82410155-428ee400-9aaa-11ea-8654-c7beae484581.png)

## Run

### on localmachine + Visual Studio

run zipkin on docker.

```shell
docker run --rm -p 9411:9411 openzipkin/zipkin
```

when open Visual Studio and Debug Run.

### on docker-compose.

```shell
docker-compose up
```

output will be.

```
Successfully built 648bca1d215a
Successfully tagged opentelemetry_app:latest
Recreating opentelemetry_app_1  ... done
Starting opentelemetry_zipkin_1 ... done
Attaching to opentelemetry_zipkin_1, opentelemetry_app_1
app_1     | Started Metrics Server on http://+:9184/metrics/
app_1     | Started Tracer Server on http://zipkin:9411/api/v2/spans
app_1     | Running and emitting metrics. Remaining time:598 seconds
app_1     | Running and emitting metrics. Remaining time:597 seconds
app_1     | Running and emitting metrics. Remaining time:596 seconds
app_1     | Running and emitting metrics. Remaining time:595 seconds
zipkin_1  |
zipkin_1  |                   oo
zipkin_1  |                  oooo
zipkin_1  |                 oooooo
zipkin_1  |                oooooooo
zipkin_1  |               oooooooooo
zipkin_1  |              oooooooooooo
zipkin_1  |            ooooooo  ooooooo
zipkin_1  |           oooooo     ooooooo
zipkin_1  |          oooooo       ooooooo
zipkin_1  |         oooooo   o  o   oooooo
zipkin_1  |        oooooo   oo  oo   oooooo
zipkin_1  |      ooooooo  oooo  oooo  ooooooo
zipkin_1  |     oooooo   ooooo  ooooo  ooooooo
zipkin_1  |    oooooo   oooooo  oooooo  ooooooo
zipkin_1  |   oooooooo      oo  oo      oooooooo
zipkin_1  |   ooooooooooooo oo  oo ooooooooooooo
zipkin_1  |       oooooooooooo  oooooooooooo
zipkin_1  |           oooooooo  oooooooo
zipkin_1  |               oooo  oooo
zipkin_1  |
zipkin_1  |      ________ ____  _  _____ _   _
zipkin_1  |     |__  /_ _|  _ \| |/ /_ _| \ | |
zipkin_1  |       / / | || |_) | ' / | ||  \| |
zipkin_1  |      / /_ | ||  __/| . \ | || |\  |
zipkin_1  |     |____|___|_|   |_|\_\___|_| \_|
zipkin_1  |
zipkin_1  | :: version 2.21.1 :: commit c30ffc5 ::
zipkin_1  |
app_1     | Running and emitting metrics. Remaining time:594 seconds
app_1     | Running and emitting metrics. Remaining time:593 seconds
zipkin_1  | 2020-05-20 05:54:33.922  INFO 1 --- [           main] z.s.ZipkinServer                         : Starting ZipkinServer on 174f1b96755f with PID 1 (/zipkin/BOOT-INF/classes started by zipkin in /zipkin)
zipkin_1  | 2020-05-20 05:54:33.982  INFO 1 --- [           main] z.s.ZipkinServer                         : The following profiles are active: shared
app_1     | Running and emitting metrics. Remaining time:591 seconds
app_1     | Running and emitting metrics. Remaining time:590 seconds
zipkin_1  | 2020-05-20 05:54:36.605  INFO 1 --- [           main] c.l.a.c.u.SystemInfo                     : Hostname: 174f1b96755f (from /proc/sys/kernel/hostname)
zipkin_1  | 2020-05-20 05:54:37.383  INFO 1 --- [oss-http-*:9411] c.l.a.s.Server                           : Serving HTTP at /0.0.0.0:9411 - http://127.0.0.1:9411/
zipkin_1  | 2020-05-20 05:54:37.388  INFO 1 --- [           main] c.l.a.s.ArmeriaAutoConfiguration         : Armeria server started at ports: {/0.0.0.0:9411=ServerPort(/0.0.0.0:9411, [http])}
app_1     | Running and emitting metrics. Remaining time:589 seconds
zipkin_1  | 2020-05-20 05:54:37.425  INFO 1 --- [           main] z.s.ZipkinServer                         : Started ZipkinServer in 9.614 seconds (JVM running for 11.298)
app_1     | Running and emitting metrics. Remaining time:588 seconds
app_1     | Running and emitting metrics. Remaining time:587 seconds
app_1     | Running and emitting metrics. Remaining time:586 seconds
app_1     | Running and emitting metrics. Remaining time:584 seconds
app_1     | Running and emitting metrics. Remaining time:583 seconds
app_1     | Running and emitting metrics. Remaining time:582 seconds
app_1     | Running and emitting metrics. Remaining time:581 seconds
```