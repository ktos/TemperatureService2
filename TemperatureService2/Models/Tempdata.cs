using System;
using System.Collections.Generic;

namespace TemperatureService2.Models
{
    public partial class Tempdata
    {
        public string Sensor { get; set; }
        public int Timestamp { get; set; }
        public float Value { get; set; }
    }
}
