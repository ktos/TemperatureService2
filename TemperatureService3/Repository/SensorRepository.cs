using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TemperatureService3.Data;
using TemperatureService3.Models;

namespace TemperatureService3.Repository
{
    public class SensorRepository : ISensorRepository
    {
        private readonly SensorsDbContext _context;

        public SensorRepository(SensorsDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Sensor> GetAllSensors()
        {
            return _context.Sensors.ToList();
        }

        public IEnumerable<Sensor> GetAllSensorsWithValues()
        {
            return _context.Sensors.Select(x => new Sensor
            {
                Description = x.Description,
                InternalId = x.InternalId,
                Name = x.Name,
                Type = x.Type,
                IsHidden = x.IsHidden,
                Values = x.Values.OrderByDescending(val => val.Timestamp).Take(50).ToList()
            }).ToList();
        }

        public Sensor GetSensor(string name)
        {
            return _context.Sensors.Include(x => x.Values).FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<GroupedTempData> GetSensorHistoryLast24Hours(string name)
        {
            return _context.SensorValues
                .Where(x => x.Sensor.Name == name)
                .Where(x => DateTime.UtcNow - x.Timestamp < TimeSpan.FromHours(24))
                .GroupBy(x => new { x.Timestamp.Hour, x.Timestamp.Minute })
                .Select(x => new GroupedTempData
                {
                    Timestamp = new DateTime(0, 0, 0, x.Key.Hour, x.Key.Minute, 0),
                    Value = x.Average(y => y.Data)
                }).ToList();
        }

        public bool UpdateSensor(SensorDto sensorDto)
        {
            var sensor = GetSensor(sensorDto.Name);
            if (sensorDto.Description != sensor.Description) sensor.Description = sensorDto.Description;
            if (sensorDto.Id != sensor.InternalId) sensor.InternalId = sensorDto.Id;
            if (sensorDto.Type != sensor.Type) sensor.Type = sensorDto.Type;
            _context.SaveChanges();

            if (sensorDto.Data.CompareTo(float.NaN) != 0 && !AddSensorReading(sensorDto))
            {
                return false;
            }

            return true;
        }

        public bool AddSensor(SensorDto sensorDto)
        {
            var sensor = new Sensor
            {
                Description = sensorDto.Description,
                InternalId = sensorDto.Id,
                Name = sensorDto.Name,
                Type = sensorDto.Type
            };

            var created = _context.Sensors.Add(sensor);
            _context.SaveChanges();

            if (sensorDto.Data.CompareTo(float.NaN) != 0 && !AddSensorReading(sensorDto))
            {
                return false;
            }

            return true;
        }

        public bool AddSensorReading(SensorDto sensorDto)
        {
            if (IsDataInvalid(sensorDto.Data))
                return false;

            _context.SensorValues.Add(new SensorValue
            {
                Data = sensorDto.Data,
                Sensor = GetSensor(sensorDto.Name),
                Timestamp = DateTime.UtcNow
            });

            _context.SaveChanges();
            return true;
        }

        private bool IsDataInvalid(float data)
        {
            return data.CompareTo(-127.0f) == 0 || data.CompareTo(85.0f) == 0;
        }
    }
}