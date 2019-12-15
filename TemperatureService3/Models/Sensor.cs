using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        /// Internal database sensor id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Publicly-visible sensor name
        /// </summary>
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