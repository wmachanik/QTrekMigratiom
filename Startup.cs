using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using QTrek.Source;
using QTrek.Source.Interfaces;
using QTrek.Source.Repositories;
using QTrek.Target;
using QTrek.Target.Interfaces;
using QTrek.Target.Repositories;
using QTrek.Tools;
using QTrek.WebFrontEnd.Data;

namespace QTrek.WebFrontEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddDbContext<SourceDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SourceConnection"));
            });
            services.AddDbContext<TargetDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("TargetConnection"));
                //                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableDetailedErrors(true);
//---- Only use if needed                options.EnableSensitiveDataLogging(true);
            });

            services.AddScoped<ISourceUnitOfWork, SourceUnitOfWork>();
            //            services.AddScoped<ITargetMachineConditionRepository, TargetMachineConditionRepository>();

            services.AddTransient<ITargetUnitOfWork, TargetUnitOfWork>();


//            LogManager.Configuration = LogManager.Configuration.Reload();
            services.AddLogging( nlogoptions => {
                nlogoptions.AddNLog("nlog.config");
            });
            services.AddLogging();
            services.AddSingleton<ILoggerManager, LoggerManager>();
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
