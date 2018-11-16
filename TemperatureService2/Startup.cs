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
using TemperatureService2.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TemperatureService2.Data;
using TemperatureService2.Services;
using Microsoft.Net.Http.Headers;
using TemperatureService2.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace TemperatureService2
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

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

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiKey", policy =>
                    policy.Requirements.Add(new ApiKeyRequirement(Configuration["ApiKey"])));
            });

            services.AddSingleton<IAuthorizationHandler, ApiKeyHandler>();
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
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "sensor",
                    template: "{name}.html",
                    defaults: new { controller = "Home", action = "Sensor" });

                routes.MapRoute(
                    name: "sensorinanotherformat",
                    template: "{name}.{format}",
                    defaults: new { controller = "Home", action = "SensorInAnotherFormat" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}