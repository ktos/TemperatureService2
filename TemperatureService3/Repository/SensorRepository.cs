﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using TemperatureService3.Data;
using TemperatureService3.Models;
using TemperatureService3.PublicDto;

namespace TemperatureService3.Repository
{
    public class SensorRepository : ISensorRepository
    {
        private readonly SensorsDbContext _context;
        private readonly IMemoryCache _cache;

        public SensorRepository(SensorsDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public IEnumerable<Sensor> GetAllSensors()
        {
            return _context.Sensors.ToList();
        }

        public IEnumerable<Sensor> GetAllSensorsWithLastValues()
        {
            if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                return _context.Sensors.Where(x => !x.IsHidden).ToList().Select(x => new Sensor
                {
                    Description = x.Description,
                    InternalId = x.InternalId,
                    Name = x.Name,
                    Type = x.Type,
                    IsHidden = x.IsHidden,
                    Values = _context.SensorValues.Where(v => v.Sensor.Name == x.Name).OrderByDescending(x => x.Timestamp).Take(1).ToList()
                }).ToList();
            }

            var lastSensorValues = _context.SensorValues.FromSqlRaw("SELECT `SensorValues`.`Id`, `Data`, `Timestamp`, `SensorId` FROM `SensorValues` WHERE `SensorValues`.`Id` IN (SELECT MAX(`SensorValues`.`Id`) FROM `SensorValues` GROUP BY `SensorId`)").Include(s => s.Sensor).AsNoTracking().ToList();

            return _context.Sensors.Where(x => !x.IsHidden).ToList().Select(x => new Sensor
            {
                Description = x.Description,
                InternalId = x.InternalId,
                Name = x.Name,
                Type = x.Type,
                IsHidden = x.IsHidden,
                Values = lastSensorValues.Where(v => v.Sensor.Name == x.Name).ToList()
            }).ToList();
        }

        public Sensor GetSensor(string name)
        {
            if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                return _context.Sensors.Include(s => s.Values).FirstOrDefault(x => x.Name == name);
            }

            var sensor = _context.Sensors.FirstOrDefault(x => x.Name == name);

            sensor.Values = _context.SensorValues.FromSqlInterpolated($"SELECT `Id`, `Data`, `SensorId`, `Timestamp` FROM `SensorValues` WHERE `SensorId` = {sensor.Id} ORDER BY `Timestamp` DESC LIMIT 10").ToList();

            return sensor;
        }

        public IEnumerable<GroupedByDateTime> GetSensorHistoryLast24Hours(string name)
        {
            IEnumerable<GroupedByDateTime> result;

            string key = nameof(GetSensorHistoryLast24Hours) + name;
            if (!_cache.TryGetValue(key, out result))
            {
                var dt = DateTime.UtcNow.AddHours(-24);
                var now = DateTime.UtcNow;

                var grouped = _context.SensorValues.AsNoTracking()
                    .Where(x => x.Sensor.Name == name)
                    .Where(x => x.Timestamp > dt)
                    .AsEnumerable()
                    .GroupBy(x => new { x.Timestamp.ToLocalTime().DayOfYear, x.Timestamp.ToLocalTime().Hour })
                    .ToList();

                result = grouped.Select(x => new GroupedByDateTime
                {
                    Timestamp = DateTimeFromDayOfYear(x.Key.DayOfYear).AddHours(x.Key.Hour),
                    Value = x.Average(y => y.Data)
                }).ToList();

                _cache.Set(key, result, DateTimeOffset.Now.AddMinutes(30));
            }

            return result;
        }

        private DateTime DateTimeFromDayOfYear(int dayOfYear)
        {
            int year = DateTime.Now.Year;
            return new DateTime(year, 1, 1).AddDays(dayOfYear - 1);
        }

        public IEnumerable<GroupedByDateTime> GetSensorHistoryLastDays(string name, int days)
        {
            IEnumerable<SensorValue> rawLastYearSensorValues = GetRawLastYearSensorValues(name);

            var dt = DateTime.UtcNow.AddDays(-days);

            return rawLastYearSensorValues
                .Where(x => x.Timestamp > dt)
                .GroupBy(x => new { x.Timestamp.Day, x.Timestamp.Month, x.Timestamp.Year })
                .Select(x => new GroupedByDateTime
                {
                    Timestamp = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day),
                    Value = x.Average(y => y.Data)
                }).OrderBy(x => x.Timestamp).ToList();
        }

        private IEnumerable<SensorValue> GetRawLastYearSensorValues(string name)
        {
            IEnumerable<SensorValue> rawLastYearSensorValues;

            string key = nameof(GetRawLastYearSensorValues) + name;
            if (!_cache.TryGetValue(key, out rawLastYearSensorValues))
            {
                var yearAgo = DateTime.UtcNow.AddDays(-365);

                rawLastYearSensorValues = _context.SensorValues.AsNoTracking()
                    .Where(x => x.Sensor.Name == name)
                    .Where(x => x.Timestamp > yearAgo)
                    .OrderBy(x => x.Timestamp)
                    .ToList();

                _cache.Set(key, rawLastYearSensorValues, DateTimeOffset.Now.AddHours(2));
            }

            return rawLastYearSensorValues;
        }

        public IEnumerable<GroupedByDateTime> GetSensorHistoryLastYear(string name)
        {
            return GetRawLastYearSensorValues(name)
                    .GroupBy(x => new { x.Timestamp.Month, x.Timestamp.Year })
                    .Select(x => new GroupedByDateTime
                    {
                        Timestamp = new DateTime(x.Key.Year, x.Key.Month, 1),
                        Value = x.Average(y => y.Data)
                    }).OrderBy(x => x.Timestamp).ToList();
        }

        public bool UpdateSensor(SensorDto sensorDto)
        {
            var sensor = GetSensor(sensorDto.Name);
            if (sensorDto.Description != null && sensorDto.Description != sensor.Description) sensor.Description = sensorDto.Description;
            if (sensorDto.InternalId != null && sensorDto.InternalId != sensor.InternalId) sensor.InternalId = sensorDto.InternalId;
            if (sensorDto.Type.HasValue && sensorDto.Type.Value != sensor.Type) sensor.Type = sensorDto.Type.Value;
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
                InternalId = sensorDto.InternalId,
                Name = sensorDto.Name,
                Type = sensorDto.Type ?? SensorType.Temperature
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