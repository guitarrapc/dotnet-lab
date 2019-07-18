using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplicationEF.Data;
using WebApplicationEF.UseCases;

namespace WebApplicationEF
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var connection = Configuration.GetConnectionString("BloggingDatabase");

            // do not use raw DbContext, but use DbContext Pooling for higher performance and low impact to DB Server.
            // ref: https://docs.microsoft.com/ja-jp/ef/core/what-is-new/ef-core-2.0
            // No pooling
            // services.AddDbContext<BloggingContext>(options => options.UseSqlServer(connection));

            // Pooling
            // basic: https://github.com/aspnet/EntityFrameworkCore/issues/10169
            // optimization: https://rehansaeed.com/optimally-configuring-entity-framework-core/
            var connectionBuilder = new SqlConnectionStringBuilder(connection)
            {
                ConnectRetryCount = 5,
                ConnectRetryInterval = 2,
                MaxPoolSize = 500, // default 128 connections
                MinPoolSize = 100,
            };
            services.AddDbContextPool<BloggingContext>(optionBuilder => optionBuilder.UseSqlServer(connectionBuilder.ConnectionString,
                options => options.EnableRetryOnFailure())
                .ConfigureWarnings(x => x.Throw(RelationalEventId.QueryClientEvaluationWarning))
                .EnableSensitiveDataLogging(_environment.IsDevelopment())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            // DI
            services.AddTransient<IBlogUseCase, UserUseCase>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
