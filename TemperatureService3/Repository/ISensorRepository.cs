using System.Collections.Generic;
using TemperatureService3.Models;

namespace TemperatureService3.Repository
{
    public interface ISensorRepository
    {
        IEnumerable<Sensor> GetAllSensors();

        IEnumerable<Sensor> GetAllSensorsWithValues();

        Sensor GetSensor(string name);

        IEnumerable<GroupedTempData> GetSensorHistoryLast24Hours(string name);

        bool UpdateSensor(SensorDto sensorDto);

        bool AddSensorReading(SensorDto sensorDto);

        bool AddSensor(SensorDto sensorDto);
    }
}