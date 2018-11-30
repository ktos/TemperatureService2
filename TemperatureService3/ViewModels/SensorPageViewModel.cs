using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperatureService3.Models;

namespace TemperatureService3.ViewModels
{
    public class SensorPageViewModel
    {
        public SensorViewModel Sensor { get; set; }
        public IEnumerable<GroupedByHours> Last24Hours { get; set; }
        public IEnumerable<GroupedByDateTime> LastWeek { get; set; }
        public IEnumerable<GroupedByDateTime> LastMonth { get; set; }
    }
}