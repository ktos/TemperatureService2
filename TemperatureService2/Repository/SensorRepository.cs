using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperatureService2.Data;
using TemperatureService2.Models;

namespace TemperatureService2.Repository
{
    public class SensorRepository : ISensorRepository
    {
        private readonly SensorsDbContext _context;

        public SensorRepository(SensorsDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Sensor> GetAllSensorsWithValues()
        {
            return _context.Sensors.Include(x => x.Values).ToList();
        }

        public IEnumerable<Sensor> GetAllSensors()
        {
            return _context.Sensors.ToList();
        }

        public Sensor GetSensor(string name)
        {
            return _context.Sensors.Include(x => x.Values).FirstOrDefault(x => x.Name == name);
        }

        public bool UpdateSensor(SensorDto sensorDto)
        {
            var sensor = GetSensor(sensorDto.Name);
            if (sensorDto.Description != sensor.Description) sensor.Description = sensorDto.Description;
            if (sensorDto.Id != sensor.InternalId) sensor.InternalId = sensorDto.Id;
            if (sensorDto.Type != sensor.Type) sensor.Type = sensorDto.Type;
            if (sensorDto.Data != -127)
            {
                AddSensorReading(sensorDto);
            }

            _context.SaveChanges();
            return true;
        }

        public bool AddSensor(SensorDto dto)
        {
            var sensor = new Sensor
            {
                Description = dto.Description,
                InternalId = dto.Id,
                Name = dto.Name,
                Type = dto.Type
            };

            var created = _context.Sensors.Add(sensor);
            if (dto.Data != float.NaN)
            {
                if (!AddSensorReading(dto))
                {
                    return false;
                }
            }

            return true;
        }

        public bool AddSensorReading(SensorDto dto)
        {
            _context.SensorValues.Add(new SensorValue
            {
                Data = dto.Data,
                Sensor = GetSensor(dto.Name),
                Timestamp = DateTime.UtcNow
            });

            _context.SaveChanges();
            return true;
        }
    }
}