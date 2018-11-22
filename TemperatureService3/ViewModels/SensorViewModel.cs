using System;
using System.Linq;
using TemperatureService3.Models;

namespace TemperatureService3.ViewModels
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

        /// <summary>
        /// Creates a new SensorViewModel based on the existing Sensor
        /// </summary>
        /// <param name="sensor">Base of new SensorViewModel</param>
        /// <returns>SensorViewModel with all data taken from the Sensor, including values</returns>
        public static SensorViewModel FromSensor(Sensor sensor)
        {
            var result = new SensorViewModel
            {
                Name = sensor.Name,
                Id = sensor.InternalId,
                Description = sensor.Description,
                Type = sensor.Type,
            };

            var newestValue = sensor.Values?.OrderByDescending(x => x.Timestamp).FirstOrDefault();
            if (newestValue != null)
            {
                result.Data = newestValue.Data;
                result.LastUpdated = newestValue.Timestamp;

                result.Status = DateTime.UtcNow - newestValue.Timestamp < TimeSpan.FromMinutes(60);
            }
            else
            {
                result.Data = float.NaN;
                result.Status = false;
            }

            return result;
        }
    }
}