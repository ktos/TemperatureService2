using System;
using System.Collections.Generic;
using System.Linq;
using TemperatureService3.Data;
using TemperatureService3.Models;

namespace TemperatureService3.Test.Helpers
{
    public class LabelsData
    {
        public string[] labels { get; set; }
        public float[] data { get; set; }
    }

    public static class Utilities
    {
        public static void InitializeDbForTests(SensorsDbContext db)
        {
            List<Sensor> sensors = GenerateSensors();
            db.Sensors.AddRange(sensors);
            db.SaveChanges();

            var values = GenerateSensorValues(sensors);
            db.SensorValues.AddRange(values);
            db.SaveChanges();
        }

        public static List<Sensor> GenerateSensors()
        {
            return new List<Sensor>()
            {
                new Sensor { Name = "outdoor", Description = "zewnętrzny", InternalId = "1", Type = SensorType.Temperature },
                new Sensor { Name = "indoor", Description = "wewnętrzny", InternalId = "2", Type = SensorType.Temperature },
                new Sensor { Name = "soil", Description = "wilgotność gleby", InternalId = "3", Type = SensorType.SoilHumidity }
            };
        }

        public static List<SensorValue> GenerateSensorValues(IEnumerable<Sensor> sensors)
        {
            var list = new List<SensorValue>();

            foreach (var s in sensors)
            {
                for (int i = 0; i < 100; i++)
                {
                    list.Add(new SensorValue { Data = 3.14f, Sensor = s, Timestamp = DateTime.UtcNow - TimeSpan.FromMinutes(i * 15) });
                }
            }

            return list;
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static float RandomFloat()
        {
            return (float)random.NextDouble();
        }
    }
}