﻿using System.Collections.Generic;
using TemperatureService2.Models;

namespace TemperatureService2.Repository
{
    public interface ISensorRepository
    {
        IEnumerable<Sensor> GetAllSensors();

        IEnumerable<Sensor> GetAllSensorsWithValues();

        Sensor GetSensor(string name);

        void UpdateSensor(SensorDto sensorDto);

        void AddSensorReading(SensorDto dto);

        void AddSensor(SensorDto dto);
    }
}