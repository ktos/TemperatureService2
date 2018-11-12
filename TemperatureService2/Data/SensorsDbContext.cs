using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperatureService2.Models;

namespace TemperatureService2.Data
{
    public class SensorsDbContext : DbContext
    {
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorValue> SensorValues { get; set; }

        public SensorsDbContext(DbContextOptions<SensorsDbContext> options) : base(options)
        {
        }
    }
}
