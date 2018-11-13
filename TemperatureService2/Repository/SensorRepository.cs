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
    }
}
