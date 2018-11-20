using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using TemperatureService3.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TemperatureService3.Data;
using TemperatureService3.Services;
using Microsoft.Net.Http.Headers;
using TemperatureService3.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace TemperatureService3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SensorsDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ISensorRepository, SensorRepository>();

            services.AddMvc(options =>
            {
                options.FormatterMappings.SetMediaTypeMappingForFormat("wns", MediaTypeHeaderValue.Parse("application/xml"));
                options.OutputFormatters.Add(new WnsOutputFormatter());
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationDefaults.AuthenticationScheme;
            })
                .AddApiKeyAuthentication(options => options.ApiKey = Configuration["ApiKey"]);
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "sensor-html",
                    template: "{name}.html",
                    defaults: new { controller = "Home", action = "Sensor" });

                routes.MapRoute(
                    name: "sensor-anotherformat",
                    template: "{name}.{format}",
                    defaults: new { controller = "Home", action = "SensorInAnotherFormat" });

                routes.MapRoute(
                    name: "sensor",
                    template: "{name}",
                    defaults: new { controller = "Home", action = "Sensor" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}