using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TemperatureService3.Models
{
    /// <summary>
    /// Possible supported types of sensors
    /// </summary>
    public enum SensorType
    {
        Temperature,
        SoilHumidity
    }

    /// <summary>
    /// The class representing single device/type object sending data
    /// to the system
    /// </summary>
    public class Sensor
    {
        /// <summary>
        /// Publicly-visible sensor name, which will be used to
        /// refer to id
        /// </summary>
        [Key]
        public string Name { get; set; }

        /// <summary>
        /// Internal ID of the sensor, e.g. from a distributor
        /// or I2C identifier
        /// </summary>
        public string InternalId { get; set; }

        /// <summary>
        /// Sensor description (e.g. location)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of the sensor
        /// </summary>
        public SensorType Type { get; set; }

        /// <summary>
        /// Is the sensor shown on the dashboard
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Collection of historical values received from this sensor
        /// </summary>
        public virtual IEnumerable<SensorValue> Values { get; set; }
    }
}