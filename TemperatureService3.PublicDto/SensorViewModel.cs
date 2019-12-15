using System;
using System.Collections.Generic;
using System.Text;

namespace TemperatureService3.PublicDto
{
    /// <summary>
    /// Describes trend between current and previous values
    /// </summary>
    public enum Difference
    {
        Unknown,
        Steady,
        Lowering,
        Rising
    }

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
        /// What is the difference between latest and previous data:
        /// is it rising, lowering or staying in the same level
        /// </summary>
        public Difference Trend { get; set; }
    }
}