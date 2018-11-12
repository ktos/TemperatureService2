using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void Migrate()
        {
            foreach (var s in _tempdata.Tempdata.Select(x => x.Sensor).Distinct())
            {
                _tempdata.Tempdata.Where(x => x.Sensor == s)
                    .ToList()
                    .ForEach(x => _sensors.SensorValues.Add(new Models.SensorValue
                    {
                        Data = x.Value,
                        Sensor = _sensors.Sensors.FirstOrDefault(y => y.Name == x.Sensor),
                        Timestamp = DateTimeOffset.FromUnixTimeSeconds(x.Timestamp).UtcDateTime
                    }));
            }

            _sensors.SaveChanges();
        }
    }
}
