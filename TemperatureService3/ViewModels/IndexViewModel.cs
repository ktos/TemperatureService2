using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperatureService3.Models;
using TemperatureService3.PublicDto;

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
                if (!item.IsHidden)
                    Sensors.Add(SensorViewModelFactory.FromSensor(item));
            }
        }
    }
}