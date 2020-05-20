using Microsoft.Extensions.Configuration;
using OpenTelemetry.Exporter.Prometheus;
using OpenTelemetry.Metrics.Configuration;
using OpenTelemetry.Metrics.Export;
using OpenTelemetry.Trace;
using OpenTelemetry.Trace.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OpenTelemetrySample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var exporterEndpoint = config.GetValue<string>("MeterExporterEndpoint", "http://localhost:9184/metrics/");
            var exporterHostingEndpoint = config.GetValue<string>("MeterExporterHostingEndpoint", "http://localhost:9184/metrics/");
            var tracerEndpoint = config.GetValue<string>("TracerExporterEndpoint", "http://localhost:9411/api/v2/spans");

            // MetricsServer for Prometheus pull model
            var exporter = new PrometheusExporter(new PrometheusExporterOptions() { Url = exporterEndpoint });
            var metricsServer = new PrometheusExporterMetricsHttpServerCustom(exporter, exporterHostingEndpoint);
            metricsServer.Start();
            Console.WriteLine($"Started Metrics Server on {exporterEndpoint}");

            /*
            Following is sample prometheus.yml config. Adjust port,interval as needed.
            scrape_configs:
              # The job name is added as a label `job=<job_name>` to any timeseries scraped from this config.
              - job_name: 'OpenTelemetryTest'
                # metrics_path defaults to '/metrics'
                # scheme defaults to 'http'.
                static_configs:
                - targets: ['localhost:9184']
            */

            // Metrics (factory is cacheable)
            var processor = new UngroupedBatcher();
            var spanContext = default(SpanContext);
            var meterFactory = MeterFactory.Create(mb =>
            {
                mb.SetMetricProcessor(processor);
                mb.SetMetricExporter(exporter);
                mb.SetMetricPushInterval(TimeSpan.FromSeconds(10));
            });
            var meter = meterFactory.GetMeter("Sample");
            var counter = meter.CreateInt64Counter("sample/measure/initialize");

            var labels1 = new List<KeyValuePair<string, string>>();
            labels1.Add(new KeyValuePair<string, string>("dim1", "value1"));

            var labels2 = new List<KeyValuePair<string, string>>();
            labels2.Add(new KeyValuePair<string, string>("dim2", "value2"));

            // TracerServer for Zipkin push model (in case you won't run on docker)
            // $ docker run --rm -p 9411:9411 openzipkin/zipkin
            // Tracer (factory is cacheable)
            var traceRandom = new Random();
            var frontTracerFactory = TracerFactory.Create(builder => builder.UseZipkin(o =>
            {
                o.ServiceName = "front-zipkin";
                o.Endpoint = new Uri(tracerEndpoint);
            }));
            var grpcTracerFactory = TracerFactory.Create(builder => builder.UseZipkin(o =>
            {
                o.ServiceName = "grpc-zipkin";
                o.Endpoint = new Uri(tracerEndpoint);
            }));
            Console.WriteLine($"Started Tracer Server on {tracerEndpoint}");

            // Execute
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalMinutes < 10)
            {
                // metrics
                counter.Add(spanContext, 100, meter.GetLabelSet(labels1));

                // tracer
                await ExecuteTrace(frontTracerFactory, grpcTracerFactory, traceRandom);

                await Task.Delay(1000);
                var remaining = (10 * 60) - sw.Elapsed.TotalSeconds;
                Console.WriteLine("Running and emitting metrics. Remaining time:" + (int)remaining + " seconds");
            }

            frontTracerFactory?.Dispose();
            grpcTracerFactory?.Dispose();

            metricsServer.Stop();
        }

        private static async Task ExecuteTrace(TracerFactory frontTracerFactory, TracerFactory grpcTracerFactory, Random traceRandom)
        {
            var taskList = new List<Task>();

            // service A
            var tracer = frontTracerFactory.GetTracer("web");
            using (tracer.StartActiveSpan($"web", out var parent))
            {
                tracer.CurrentSpan.SetAttribute("key", 123);
                tracer.CurrentSpan.AddEvent("test-event");

                await Task.Delay(TimeSpan.FromMilliseconds(traceRandom.Next(20, 100)));

                // service B
                var tracerGrpc = grpcTracerFactory.GetTracer("grpc");
                var tracerGrpcRedis = grpcTracerFactory.GetTracer("redis");
                var tracerGrpcDb = grpcTracerFactory.GetTracer("db");
                using (tracerGrpc.StartActiveSpan("grpc", out var grpc))
                {
                    grpc.SetAttribute("path", "/api/user/status");

                    using (tracerGrpcRedis.StartActiveSpan("redis", out var redis))
                    {
                        redis.SetAttribute("key", "uid-123");
                        redis.SetAttribute("status", "up");
                        var t1 = Task.Delay(TimeSpan.FromMilliseconds(traceRandom.Next(1, 20)));
                        taskList.Add(t1);

                        using (tracerGrpcDb.StartActiveSpan("db", out var db))
                        {
                            db.SetAttribute("database", "user");
                            db.SetAttribute("table", "status");
                            db.SetAttribute("uid", "123");
                            var t2 = Task.Delay(TimeSpan.FromMilliseconds(traceRandom.Next(2, 50)));
                            taskList.Add(t2);
                        }
                    }

                    await Task.WhenAll(taskList);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(traceRandom.Next(10, 50)));
            }
        }

        private static readonly ConcurrentDictionary<Guid, string> Responses = new ConcurrentDictionary<Guid, string>();
    }
}
