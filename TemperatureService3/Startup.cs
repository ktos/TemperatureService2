using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using StackExchange.Profiling;
using TemperatureService3.Authentication;
using TemperatureService3.Data;
using TemperatureService3.Repository;
using TemperatureService3.Services;

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
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection"))));

            services.AddMiniProfiler()
                .AddEntityFramework();

            services.AddHealthChecks()
                .AddMySql(Configuration.GetConnectionString("DefaultConnection"))
                .AddDbContextCheck<SensorsDbContext>();

            services.AddScoped<ISensorRepository, SensorRepository>();
            services.AddTransient<IAppVersionService, AppVersionService>();

            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddMvc(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.FormatterMappings.SetMediaTypeMappingForFormat("wns", MediaTypeHeaderValue.Parse("application/xml"));
                options.OutputFormatters.Add(new WnsOutputFormatter());
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationDefaults.AuthenticationScheme;
            })
                .AddApiKeyAuthentication(options => options.ApiKey = Configuration["ApiKey"]);

            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/ErrorCode", "?code={0}");
            app.UseStaticFiles();

            app.UseMiniProfiler();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "sensor-html",
                    pattern: "{name}.html",
                    defaults: new { controller = "Home", action = "Sensor" });

                endpoints.MapControllerRoute(
                    name: "sensor-anotherformat",
                    pattern: "{name}.{format}",
                    defaults: new { controller = "Home", action = "SensorInAnotherFormat" });

                endpoints.MapControllerRoute(
                    name: "sensor",
                    pattern: "{name}",
                    defaults: new { controller = "Home", action = "Sensor" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHealthChecks("/health");

                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}