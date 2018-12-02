﻿using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<GroupedByHours> GetSensorHistoryLast24Hours(string name)
        {
            var dt = DateTime.UtcNow.AddHours(-24);

            var grouped = _context.SensorValues
                .Where(x => x.Sensor.Name == name)
                .Where(x => x.Timestamp > dt)
                .OrderBy(x => x.Timestamp)
                .GroupBy(x => new { x.Timestamp.Hour })
                .ToList();

            return grouped.Select(x => new GroupedByHours
            {
                Hour = x.Key.Hour,
                Value = x.Average(y => y.Data)
            }).ToList();
        }

        public IEnumerable<GroupedByDateTime> GetSensorHistoryLastDays(string name, int days)
        {
            var dt = DateTime.UtcNow.AddDays(-days);

            var grouped = _context.SensorValues
                .Where(x => x.Sensor.Name == name)
                .Where(x => x.Timestamp > dt)
                .OrderBy(x => x.Timestamp)
                .GroupBy(x => new { x.Timestamp.Day, x.Timestamp.Month, x.Timestamp.Year })
                .ToList();

            return grouped.Select(x => new GroupedByDateTime
            {
                Timestamp = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day),
                Value = x.Average(y => y.Data)
            }).OrderBy(x => x.Timestamp).ToList();
        }

        public IEnumerable<GroupedByDateTime> GetSensorHistoryLastYear(string name)
        {
            var dt = DateTime.UtcNow.AddDays(-365);

            var grouped = _context.SensorValues
                .Where(x => x.Sensor.Name == name)
                .Where(x => x.Timestamp > dt)
                .OrderBy(x => x.Timestamp)
                .GroupBy(x => new { x.Timestamp.Month, x.Timestamp.Year })
                .ToList();

            return grouped.Select(x => new GroupedByDateTime
            {
                Timestamp = new DateTime(x.Key.Year, x.Key.Month, 1),
                Value = x.Average(y => y.Data)
            }).OrderBy(x => x.Timestamp).ToList();
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