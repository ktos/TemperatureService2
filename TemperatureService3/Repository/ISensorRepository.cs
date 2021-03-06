﻿using System.Collections.Generic;
using TemperatureService3.Models;

namespace TemperatureService3.Repository
{
    public interface ISensorRepository
    {
        IEnumerable<Sensor> GetAllSensors();

        IEnumerable<Sensor> GetAllSensorsWithLastValues();

        Sensor GetSensor(string name);

        IEnumerable<GroupedByDateTime> GetSensorHistoryLast24Hours(string name);

        IEnumerable<GroupedByDateTime> GetSensorHistoryLastDays(string name, int days);

        IEnumerable<GroupedByDateTime> GetSensorHistoryLastYear(string name);

        bool UpdateSensor(SensorDto sensorDto);

        bool AddSensorReading(SensorDto sensorDto);

        bool AddSensor(SensorDto sensorDto);
    }
}