using System;

namespace TemperatureService3.Models
{
    public class GroupedByDateTime
    {
        public DateTime Timestamp { get; set; }
        public float Value { get; set; }
    }

    public class GroupedByHours
    {
        public int Hour { get; set; }
        public float Value { get; set; }
    }
}