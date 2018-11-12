using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TemperatureService2.Data;
using TemperatureService2.Migrations;

namespace TemperatureService2
{
    public static class DatabaseMigrationExtensions
    {
        public static IWebHost MigrateToV3(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                try
                {
                    var migrationInitializer = new DatabaseMigrationInitializer();
                    migrationInitializer.MigrateDatabase(serviceProvider);
                }
                catch (Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogCritical(ex, "Unable to migrate the database");
                }
            }

            return host;
        }
    }

    public class DatabaseMigrationInitializer
    {
        public void MigrateDatabase(IServiceProvider serviceProvider)
        {
            using (var td = serviceProvider.GetRequiredService<TempdataDbContext>())
            using (var sd = serviceProvider.GetRequiredService<SensorsDbContext>())
            {
                if (!sd.Sensors.Any())
                {
                    var migrator = new TempdataSensorsMigrator(sd, td);
                    migrator.MigrateSensors();
                }

                if (!sd.SensorValues.Any())
                {
                    var migrator = new TempdataSensorsMigrator(sd, td);
                    migrator.MigrateValues();
                }
            }
        }
    }
}