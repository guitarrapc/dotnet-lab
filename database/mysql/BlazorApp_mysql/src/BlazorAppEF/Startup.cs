using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorAppEF.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using BlazorAppEF.Services;
using System.Net.Http;
using BlazorAppEF.Models;

namespace BlazorAppEF
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Pooling
            var connection = Configuration.GetConnectionString("BloggingDatabase");
            // basic: https://github.com/aspnet/EntityFrameworkCore/issues/10169
            // optimization: https://rehansaeed.com/optimally-configuring-entity-framework-core/
            var connectionBuilder = new SqlConnectionStringBuilder(connection)
            {
                MaxPoolSize = 128, // default 128 connections
                MinPoolSize = 100,
            };
            services.AddDbContextPool<BloggingContext>(optionBuilder => optionBuilder.UseMySql(connectionBuilder.ConnectionString)
                .EnableSensitiveDataLogging(_environment.IsDevelopment())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            // models
            services.AddTransient<BlogModel>();

            // services
            services.AddTransient<SystemService>();
            services.AddTransient<BlogService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
