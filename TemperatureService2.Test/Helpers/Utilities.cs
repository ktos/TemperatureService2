using System;
using System.Collections.Generic;
using TemperatureService2.Data;
using TemperatureService2.Models;

namespace TemperatureService2.Test.Helpers
{
    public static class Utilities
    {
        public static void InitializeDbForTests(SensorsDbContext db)
        {
            List<Sensor> sensors = GetSensors();
            db.Sensors.AddRange(sensors);
            db.SaveChanges();

            db.SensorValues.AddRange(GetSensorValues(sensors));
            db.SaveChanges();
        }

        public static List<Sensor> GetSensors()
        {
            return new List<Sensor>()
            {
                new Sensor { Name = "outdoor", Description = "zewnętrzny", InternalId = "1", Type = SensorType.Temperature },
                new Sensor { Name = "indoor", Description = "wewnętrzny", InternalId = "2", Type = SensorType.Temperature }
            };
        }

        public static List<SensorValue> GetSensorValues(IEnumerable<Sensor> sensors)
        {
            var list = new List<SensorValue>();
            var r = new Random();
#pragma warning disable RCS1118 // Mark local variable as const.
            var id = 1;
#pragma warning restore RCS1118 // Mark local variable as const.

            foreach (var s in sensors)
            {
                for (int i = 0; i < r.Next(10); i++)
                {
                    list.Add(new SensorValue { Data = r.Next(100), Id = id, Sensor = s, Timestamp = DateTime.UtcNow - TimeSpan.FromMinutes(i * 10) });
                    i++;
                }
            }

            return list;
        }
    }
}