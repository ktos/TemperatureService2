using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        /// Internal value id for EF Core
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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