using System;
using System.Collections.Generic;
using System.Linq;
using TemperatureService3.Data;
using TemperatureService3.Models;

namespace TemperatureService3.Test.Helpers
{
    public class LabelsData
    {
        public DateTime[] labels { get; set; }
        public float[] data { get; set; }
    }

    public static class Utilities
    {
        public static void InitializeDbForTests(SensorsDbContext db)
        {
            List<Sensor> sensors = GenerateSensors();
            db.Sensors.AddRange(sensors);
            db.SaveChanges();

            SensorValues = GenerateRandomSensorValues(sensors);
            db.SensorValues.AddRange(SensorValues);
            db.SaveChanges();
        }

        public static List<SensorValue> SensorValues;

        public static List<Sensor> GenerateSensors()
        {
            return new List<Sensor>()
            {
                new Sensor { Name = "outdoor", Description = "zewnętrzny", InternalId = "1", Type = SensorType.Temperature },
                new Sensor { Name = "indoor", Description = "wewnętrzny", InternalId = "2", Type = SensorType.Temperature }
            };
        }

        public static List<SensorValue> GenerateRandomSensorValues(IEnumerable<Sensor> sensors)
        {
            var list = new List<SensorValue>();
            var r = new Random();

            foreach (var s in sensors)
            {
                for (int i = 0; i < r.Next(10); i++)
                {
                    list.Add(new SensorValue { Data = r.Next(100), Sensor = s, Timestamp = DateTime.UtcNow - TimeSpan.FromMinutes(i * 10) });
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