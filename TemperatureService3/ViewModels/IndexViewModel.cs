using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperatureService3.Models;

namespace TemperatureService3.ViewModels
{
    public class IndexViewModel
    {
        public IList<SensorViewModel> Sensors { get; set; }

        public IndexViewModel(IEnumerable<Sensor> sensors)
        {
            Sensors = new List<SensorViewModel>();

            foreach (var item in sensors)
            {
                Sensors.Add(new SensorViewModel(item));
            }
        }
    }
}