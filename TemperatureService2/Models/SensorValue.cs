using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemperatureService2.Models
{
    /// <summary>
    /// Value received from sensor
    /// </summary>
    public class SensorValue
    {
        /// <summary>
        /// Sensor which sent the value
        /// </summary>
        public Sensor Sensor { get; set; }

        /// <summary>
        /// Value sent by the sensor
        /// </summary>
        public float Data { get; set; }

        /// <summary>
        /// Time the value was sent/received, as universal time
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
