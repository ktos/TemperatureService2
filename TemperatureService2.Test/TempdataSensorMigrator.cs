using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TemperatureService2.Data;
using TemperatureService2.Migrations;
using TemperatureService2.Models;
using Xunit;

namespace TemperatureService2.Test
{
    public class TempdataSensorMigrator
    {
        [Fact]
        public void Migrate_SensorValues()
        {
            var options = new DbContextOptionsBuilder<TempdataDbContext>().UseInMemoryDatabase("InMemoryDB");
            var options2 = new DbContextOptionsBuilder<SensorsDbContext>().UseInMemoryDatabase("InMemoryDB");

            // arrange
            using (var td = new TempdataDbContext(options.Options))
            {
                td.Tempdata.Add(new Models.Tempdata
                {
                    Sensor = "outdoor",
                    Timestamp = (int)new DateTimeOffset(new DateTime(2018, 10, 30, 10, 00, 00)).ToUnixTimeSeconds(),
                    Value = 10.0f
                });

                td.Tempdata.Add(new Models.Tempdata
                {
                    Sensor = "indoor",
                    Timestamp = (int)new DateTimeOffset(new DateTime(2018, 12, 1, 14, 12, 00)).ToUnixTimeSeconds(),
                    Value = 1.0f
                });

                td.Tempdata.Add(new Models.Tempdata
                {
                    Sensor = "outdoor",
                    Timestamp = (int)new DateTimeOffset(new DateTime(2018, 10, 30, 12, 00, 00)).ToUnixTimeSeconds(),
                    Value = 12.0f
                });

                td.Tempdata.Add(new Models.Tempdata
                {
                    Sensor = "indoor",
                    Timestamp = (int)new DateTimeOffset(new DateTime(2018, 12, 12, 14, 12, 00)).ToUnixTimeSeconds(),
                    Value = 2.0f
                });

                td.Tempdata.Add(new Models.Tempdata
                {
                    Sensor = "outdoor",
                    Timestamp = (int)new DateTimeOffset(new DateTime(2018, 12, 1, 14, 00, 00)).ToUnixTimeSeconds(),
                    Value = 14.0f
                });

                td.SaveChanges();
            }

            using (var sd = new SensorsDbContext(options2.Options))
            {
                sd.Sensors.Add(new Sensor { Name = "outdoor" });
                sd.Sensors.Add(new Sensor { Name = "indoor" });
                sd.SaveChanges();
            }

            using (var td = new TempdataDbContext(options.Options))
            using (var sd = new SensorsDbContext(options2.Options))
            {
                var t = new TempdataSensorsMigrator(sd, td);
                t.MigrateValues();

                var outdoor = sd.Sensors.Include(x => x.Values).First(x => x.Name == "outdoor");
                var indoor = sd.Sensors.Include(x => x.Values).First(x => x.Name == "indoor");

                Assert.IsType<Sensor>(outdoor);
                Assert.IsType<Sensor>(indoor);
                Assert.Equal(3, outdoor.Values.Count());
                Assert.Equal(2, indoor.Values.Count());

                var firstOutdoor = outdoor.Values.OrderBy(x => x.Timestamp).First();
                var firstIndoor = indoor.Values.OrderBy(x => x.Timestamp).First();
                var lastOutdoor = outdoor.Values.OrderByDescending(x => x.Timestamp).First();
                var lastIndoor = indoor.Values.OrderByDescending(x => x.Timestamp).First();

                Assert.Equal(10.0, firstOutdoor.Data);
                Assert.Equal(14.0, lastOutdoor.Data);
                Assert.Equal(1.0, firstIndoor.Data);
                Assert.Equal(2.0, lastIndoor.Data);

                Assert.Equal(new DateTime(2018, 10, 30, 10, 00, 00).ToUniversalTime(), firstOutdoor.Timestamp);
                Assert.Equal(new DateTime(2018, 12, 1, 14, 00, 00).ToUniversalTime(), lastOutdoor.Timestamp);

                Assert.Equal(new DateTime(2018, 12, 1, 14, 12, 00).ToUniversalTime(), firstIndoor.Timestamp);
            }
        }

        [Fact]
        public void Migrate_Sensors()
        {
            var options = new DbContextOptionsBuilder<TempdataDbContext>().UseInMemoryDatabase("InMemoryDB2");
            var options2 = new DbContextOptionsBuilder<SensorsDbContext>().UseInMemoryDatabase("InMemoryDB2");

            // arrange
            using (var td = new TempdataDbContext(options.Options))
            {
                td.Tempdata.Add(new Models.Tempdata
                {
                    Sensor = "outdoor",
                    Timestamp = (int)new DateTimeOffset(new DateTime(2018, 10, 30, 10, 00, 00)).ToUnixTimeSeconds(),
                    Value = 10.0f
                });

                td.Tempdata.Add(new Models.Tempdata
                {
                    Sensor = "indoor",
                    Timestamp = (int)new DateTimeOffset(new DateTime(2018, 12, 1, 14, 12, 00)).ToUnixTimeSeconds(),
                    Value = 1.0f
                });

                td.Tempdata.Add(new Models.Tempdata
                {
                    Sensor = "outdoor",
                    Timestamp = (int)new DateTimeOffset(new DateTime(2018, 10, 30, 12, 00, 00)).ToUnixTimeSeconds(),
                    Value = 12.0f
                });

                td.Tempdata.Add(new Models.Tempdata
                {
                    Sensor = "indoor",
                    Timestamp = (int)new DateTimeOffset(new DateTime(2018, 12, 12, 14, 12, 00)).ToUnixTimeSeconds(),
                    Value = 2.0f
                });

                td.Tempdata.Add(new Models.Tempdata
                {
                    Sensor = "outdoor",
                    Timestamp = (int)new DateTimeOffset(new DateTime(2018, 12, 1, 14, 00, 00)).ToUnixTimeSeconds(),
                    Value = 14.0f
                });

                td.SaveChanges();
            }

            using (var td = new TempdataDbContext(options.Options))
            using (var sd = new SensorsDbContext(options2.Options))
            {
                var t = new TempdataSensorsMigrator(sd, td);
                t.MigrateSensors();

                var outdoor = sd.Sensors.First(x => x.Name == "outdoor");
                var indoor = sd.Sensors.First(x => x.Name == "indoor");

                Assert.IsType<Sensor>(outdoor);
                Assert.IsType<Sensor>(indoor);
            }
        }
    }
}