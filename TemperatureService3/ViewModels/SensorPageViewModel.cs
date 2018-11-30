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
        public IEnumerable<GroupedTempData> Last24Hours { get; set; }
    }
}