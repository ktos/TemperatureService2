using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperatureService2.Models;

namespace TemperatureService2.ViewModels
{
    public class SensorViewModel
    {
        /// <summary>
        /// Publicly-visible sensor name, which will be used to
        /// refer to id
        /// </summary>        
        public string Name { get; set; }

        /// <summary>
        /// Internal ID of the sensor, e.g. from a distributor
        /// or I2C identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Sensor description (e.g. location)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of the sensor
        /// </summary>
        public SensorType Type { get; set; }

        /// <summary>
        /// Last data from this sensor
        /// </summary>
        public float Data { get; set; }

        /// <summary>
        /// The Time sensor was last updated
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Is the sensor alive?
        /// </summary>
        public bool Status { get; set; }

        public SensorViewModel(Sensor sensor)
        {
            Name = sensor.Name;
            Id = sensor.InternalId;
            Description = sensor.Description;
            Type = sensor.Type;

            var newestValue = sensor.Values?.OrderByDescending(x => x.Timestamp).FirstOrDefault();
            if (newestValue != null && DateTime.UtcNow - newestValue.Timestamp < TimeSpan.FromMinutes(60))
            {
                Data = newestValue.Data;
                LastUpdated = newestValue.Timestamp;
                Status = true;
            }
            else
            {
                Data = float.NaN;
                Status = false;
            }
        }
    }
}
