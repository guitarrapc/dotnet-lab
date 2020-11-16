using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Exporter.Prometheus;
using OpenTelemetry.Metrics;
using OpenTelemetry.Metrics.Export;
using OpenTelemetry.Trace;
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
            MeterProvider.SetDefault(Sdk.CreateMeterProviderBuilder()
                .SetProcessor(processor)
                .SetExporter(exporter)
                .SetPushInterval(TimeSpan.FromSeconds(10))
                .Build());
            var meterProvider = MeterProvider.Default;
            var meter = meterProvider.GetMeter("Sample");
            var counter = meter.CreateInt64Counter("sample/measure/initialize");

            var labels1 = new List<KeyValuePair<string, string>>();
            labels1.Add(new KeyValuePair<string, string>("dim1", "value1"));

            var labels2 = new List<KeyValuePair<string, string>>();
            labels2.Add(new KeyValuePair<string, string>("dim2", "value2"));

            // TracerServer for Zipkin push model (in case you won't run on docker)
            // $ docker run --rm -p 9411:9411 openzipkin/zipkin
            // Tracer (factory is cacheable)
            using var tracerFactory = Sdk.CreateTracerProviderBuilder()
                .AddSource("Samples.SampleClient", "Samples.SampleServer")
                .AddZipkinExporter(o =>
                {
                    o.ServiceName = "front-zipkin";
                    o.Endpoint = new Uri(tracerEndpoint);
                })
                .Build();
            using var backEndTracerFactory = Sdk.CreateTracerProviderBuilder()
                .AddSource("Samples.SampleServer.Redis", "Samples.SampleServer.Db")
                .AddZipkinExporter(o =>
                {
                    o.ServiceName = "backend-zipkin";
                    o.Endpoint = new Uri(tracerEndpoint);
                })
                .Build();
            Console.WriteLine($"Started Tracer Server on {tracerEndpoint}");

            // Execute http://0.0.0.0:19999
            using var sample = new InstrumentationWithActivitySource();
            sample.Start();

            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalMinutes < 10)
            {
                // metrics
                counter.Add(spanContext, 100, meter.GetLabelSet(labels1));

                await Task.Delay(1000);
                var remaining = (10 * 60) - sw.Elapsed.TotalSeconds;
                Console.WriteLine("Running and emitting metrics. Remaining time:" + (int)remaining + " seconds");
            }

            metricsServer.Stop();
        }
    }
}
