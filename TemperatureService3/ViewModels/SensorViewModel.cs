using System;
using System.Linq;
using TemperatureService3.Models;
using TemperatureService3.PublicDto;

namespace TemperatureService3.ViewModels
{
    public static class SensorViewModelFactory
    {
        /// <summary>
        /// Creates a new SensorViewModel based on the existing Sensor
        /// </summary>
        /// <param name="sensor">Base of new SensorViewModel</param>
        /// <returns>SensorViewModel with all data taken from the Sensor, including values</returns>
        public static SensorViewModel FromSensor(Sensor sensor)
        {
            var eps = 0.5;

            var result = new SensorViewModel
            {
                Name = sensor.Name,
                InternalId = sensor.InternalId,
                Description = sensor.Description,
                Type = sensor.Type,
                Data = float.NaN,
                Status = false,
                Trend = Difference.Unknown
            };

            var orderedValues = sensor.Values?.OrderByDescending(x => x.Timestamp);
            if (orderedValues != null)
            {
                var newestValue = orderedValues.FirstOrDefault();

                if (newestValue != null)
                {
                    result.Data = newestValue.Data;
                    result.LastUpdated = newestValue.Timestamp.ToLocalTime();
                    result.Status = DateTime.UtcNow.ToLocalTime() - newestValue.Timestamp < TimeSpan.FromMinutes(60);

                    if (orderedValues.Count() > 1)
                    {
                        var secondNewest = orderedValues.Skip(1).First();
                        if (newestValue.Data - secondNewest.Data > eps)
                            result.Trend = Difference.Rising;
                        else if (newestValue.Data - secondNewest.Data < -eps)
                            result.Trend = Difference.Lowering;
                        else
                            result.Trend = Difference.Steady;
                    }
                }
            }

            return result;
        }
    }
}