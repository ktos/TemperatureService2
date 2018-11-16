using System.Collections.Generic;
using TemperatureService2.Models;

namespace TemperatureService2.Repository
{
    public interface ISensorRepository
    {
        IEnumerable<Sensor> GetAllSensors();

        IEnumerable<Sensor> GetAllSensorsWithValues();

        Sensor GetSensor(string name);

        bool UpdateSensor(SensorDto sensorDto);

        bool AddSensorReading(SensorDto sensorDto);

        bool AddSensor(SensorDto sensorDto);
    }
}