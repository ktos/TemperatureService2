using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemperatureService3.ViewModels
{
    public class MenuViewModel
    {
        public IEnumerable<SensorViewModel> Sensors { get; set; }
        public SensorViewModel Active { get; set; }
    }
}