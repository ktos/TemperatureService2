using System.Collections.Generic;
using TemperatureService3.Models;

namespace TemperatureService3.Repository
{
    public interface ISensorRepository
    {
        IEnumerable<Sensor> GetAllSensors();

        Sensor GetSensor(string name);

        bool UpdateSensor(SensorDto sensorDto);

        bool AddSensorReading(SensorDto sensorDto);

        bool AddSensor(SensorDto sensorDto);
    }
}