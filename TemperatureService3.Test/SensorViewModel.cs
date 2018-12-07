using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TemperatureService3.Models;
using TemperatureService3.ViewModels;

namespace TemperatureService3.Test
{
    public class SensorViewModel
    {
        [Fact]
        public void SensorViewModel_StatusFalseWhenEmptyValues()
        {
            var sensor = new Sensor
            {
                Name = "outdoor",
                Description = "zewnątrz",
                InternalId = "1",
                Type = SensorType.Temperature,
                Values = null
            };

            var svm = ViewModels.SensorViewModel.FromSensor(sensor);
            Assert.False(svm.Status);
            Assert.Equal(float.NaN, svm.Data);
        }

        [Fact]
        public void SensorViewModel_StatusFalseWhenNoValues()
        {
            var sensor = new Sensor
            {
                Name = "outdoor",
                Description = "zewnątrz",
                InternalId = "1",
                Type = SensorType.Temperature,
                Values = new List<SensorValue>()
            };

            var svm = ViewModels.SensorViewModel.FromSensor(sensor);
            Assert.False(svm.Status);
            Assert.Equal(float.NaN, svm.Data);
        }

        [Fact]
        public void SensorViewModel_PositiveData()
        {
            var now = DateTime.UtcNow;

            var sensor = new Sensor
            {
                Name = "outdoor",
                Description = "zewnątrz",
                InternalId = "1",
                Type = SensorType.Temperature,
                Values = new List<SensorValue>
                {
                    new SensorValue {  Data = 1, Id = 1, Timestamp = now },
                    new SensorValue {  Data = 2, Id = 2, Timestamp = now - TimeSpan.FromMinutes(15) },
                    new SensorValue {  Data = 3, Id = 3, Timestamp = now - TimeSpan.FromMinutes(30) }
                }
            };

            var svm = ViewModels.SensorViewModel.FromSensor(sensor);
            Assert.True(svm.Status);
            Assert.Equal("outdoor", svm.Name);
            Assert.Equal("zewnątrz", svm.Description);
            Assert.Equal("1", svm.Id);
            Assert.Equal(SensorType.Temperature, svm.Type);
            Assert.Equal(1, svm.Data);
            Assert.Equal(now, svm.LastUpdated.ToUniversalTime());
        }

        [Fact]
        public void SensorViewModel_StatusFalseWhenTooOld()
        {
            var now = DateTime.UtcNow;

            var sensor = new Sensor
            {
                Name = "outdoor",
                Description = "zewnątrz",
                InternalId = "1",
                Type = SensorType.Temperature,
                Values = new List<SensorValue>
                {
                    new SensorValue {  Data = 1, Id = 1, Timestamp = now - TimeSpan.FromMinutes(60) },
                    new SensorValue {  Data = 2, Id = 2, Timestamp = now - TimeSpan.FromMinutes(75) },
                    new SensorValue {  Data = 3, Id = 3, Timestamp = now - TimeSpan.FromMinutes(90) }
                }
            };

            var svm = ViewModels.SensorViewModel.FromSensor(sensor);
            Assert.False(svm.Status);
            Assert.Equal("outdoor", svm.Name);
            Assert.Equal("zewnątrz", svm.Description);
            Assert.Equal("1", svm.Id);
            Assert.Equal(SensorType.Temperature, svm.Type);
            Assert.Equal(1, svm.Data);
            Assert.Equal(now - TimeSpan.FromMinutes(60), svm.LastUpdated.ToUniversalTime());
        }

        [Fact]
        public void SensorViewModel_DifferenceProperlyCalculated()
        {
            var now = DateTime.UtcNow;

            var sensor = new Sensor
            {
                Name = "outdoor",
                Description = "zewnątrz",
                InternalId = "1",
                Type = SensorType.Temperature,
                Values = new List<SensorValue>
                {
                    new SensorValue {  Data = 1, Id = 1, Timestamp = now - TimeSpan.FromMinutes(60) },
                    new SensorValue {  Data = 2, Id = 2, Timestamp = now - TimeSpan.FromMinutes(75) },
                    new SensorValue {  Data = 3, Id = 3, Timestamp = now - TimeSpan.FromMinutes(90) }
                }
            };

            var svm = ViewModels.SensorViewModel.FromSensor(sensor);
            Assert.Equal(Difference.Lowering, svm.DifferenceFromPrevious);

            sensor = new Sensor
            {
                Name = "outdoor",
                Description = "zewnątrz",
                InternalId = "1",
                Type = SensorType.Temperature,
                Values = new List<SensorValue>
                {
                    new SensorValue {  Data = 3, Id = 1, Timestamp = now - TimeSpan.FromMinutes(60) },
                    new SensorValue {  Data = 2, Id = 2, Timestamp = now - TimeSpan.FromMinutes(75) },
                    new SensorValue {  Data = 1, Id = 3, Timestamp = now - TimeSpan.FromMinutes(90) }
                }
            };
            svm = ViewModels.SensorViewModel.FromSensor(sensor);
            Assert.Equal(Difference.Rising, svm.DifferenceFromPrevious);

            sensor = new Sensor
            {
                Name = "outdoor",
                Description = "zewnątrz",
                InternalId = "1",
                Type = SensorType.Temperature,
                Values = new List<SensorValue>
                {
                    new SensorValue {  Data = 2, Id = 1, Timestamp = now - TimeSpan.FromMinutes(60) },
                    new SensorValue {  Data = 2, Id = 2, Timestamp = now - TimeSpan.FromMinutes(75) },
                    new SensorValue {  Data = 1, Id = 3, Timestamp = now - TimeSpan.FromMinutes(90) }
                }
            };
            svm = ViewModels.SensorViewModel.FromSensor(sensor);
            Assert.Equal(Difference.Steady, svm.DifferenceFromPrevious);

            sensor = new Sensor
            {
                Name = "outdoor",
                Description = "zewnątrz",
                InternalId = "1",
                Type = SensorType.Temperature,
                Values = new List<SensorValue>
                {
                    new SensorValue {  Data = 2.3f, Id = 1, Timestamp = now - TimeSpan.FromMinutes(60) },
                    new SensorValue {  Data = 2, Id = 2, Timestamp = now - TimeSpan.FromMinutes(75) },
                    new SensorValue {  Data = 1, Id = 3, Timestamp = now - TimeSpan.FromMinutes(90) }
                }
            };
            svm = ViewModels.SensorViewModel.FromSensor(sensor);
            Assert.Equal(Difference.Steady, svm.DifferenceFromPrevious);
        }
    }
}