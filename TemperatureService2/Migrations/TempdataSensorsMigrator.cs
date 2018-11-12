using System;
using System.Collections.Generic;
using System.Linq;
using TemperatureService2.Data;

namespace TemperatureService2.Migrations
{
    public class TempdataSensorsMigrator
    {
        private readonly SensorsDbContext _sensors;
        private readonly TempdataDbContext _tempdata;

        public TempdataSensorsMigrator(SensorsDbContext sensors, TempdataDbContext tempdata)
        {
            _sensors = sensors;
            _tempdata = tempdata;
        }

        public void MigrateSensors()
        {
            foreach (var s in _tempdata.Tempdata.Select(x => x.Sensor).Distinct())
            {
                _sensors.Sensors.Add(new Models.Sensor { Name = s, Type = Models.SensorType.Temperature });
            }

            _sensors.SaveChanges();
        }

        public void MigrateValues()
        {
            foreach (var s in _tempdata.Tempdata.Select(x => x.Sensor).Distinct())
            {
                var sensor = _sensors.Sensors.FirstOrDefault(y => y.Name == s);
                var values = new List<Models.SensorValue>();

                _tempdata.Tempdata.Where(x => x.Sensor == s)
                    .ToList()
                    .ForEach(x => values.Add(new Models.SensorValue
                    {
                        Data = x.Value,
                        Sensor = sensor,
                        Timestamp = DateTimeOffset.FromUnixTimeSeconds(x.Timestamp).UtcDateTime
                    }));

                _sensors.SensorValues.AddRange(values);
            }

            _sensors.SaveChanges();
        }
    }
}