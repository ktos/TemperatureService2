using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperatureService3.Models;
using TemperatureService3.PublicDto;

namespace TemperatureService3.ViewModels
{
    public class SensorPageViewModel
    {
        public IList<SensorViewModel> AllSensors { get; set; }
        public SensorViewModel Sensor { get; set; }
        public IEnumerable<GroupedByDateTime> Last24Hours { get; set; }
        public IEnumerable<GroupedByDateTime> LastWeek { get; set; }
        public IEnumerable<GroupedByDateTime> LastMonth { get; set; }
        public IEnumerable<GroupedByDateTime> LastYearByMonth { get; set; }
        public IEnumerable<GroupedByDateTime> LastYear { get; set; }
    }
}