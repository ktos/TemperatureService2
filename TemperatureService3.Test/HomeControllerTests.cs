using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperatureService3.Controllers;
using TemperatureService3.Models;
using TemperatureService3.PublicDto;
using TemperatureService3.Repository;
using TemperatureService3.ViewModels;
using Xunit;

namespace TemperatureService3.Test
{
    public class HomeControllerTests
    {
        private readonly Mock<ISensorRepository> sensorsRepoMock;

        private readonly HomeController controller;

        public HomeControllerTests()
        {
            sensorsRepoMock = new Mock<ISensorRepository>();
            controller = new HomeController(sensorsRepoMock.Object);
        }

        [Fact]
        public void Index_ReturnsAllSensorsWithoutValues()
        {
            // Arrange
            var mockTempdataList = new List<Sensor>
            {
                new Sensor { Name = "outdoor", Description = "zewnêtrzny", InternalId = "1", Type = SensorType.Temperature },
                new Sensor { Name = "indoor", Description = "wewnêtrzny", InternalId = "2", Type = SensorType.SoilHumidity }
            };

            sensorsRepoMock
                .Setup(repo => repo.GetAllSensorsWithLastValues())
                .Returns(mockTempdataList);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Dashboard", viewResult.ViewData["Title"]);
            Assert.Equal(2, model.Sensors.Count());
            Assert.Equal(float.NaN, model.Sensors.First().Data);
        }

        [Fact]
        public void Index_ReturnsAllSensorsWithoutValuesAndHidden()
        {
            // Arrange
            var mockTempdataList = new List<Sensor>
            {
                new Sensor { Name = "outdoor", Description = "zewnêtrzny", InternalId = "1", Type = SensorType.Temperature, IsHidden = true },
                new Sensor { Name = "indoor", Description = "wewnêtrzny", InternalId = "2", Type = SensorType.SoilHumidity }
            };

            sensorsRepoMock
                .Setup(repo => repo.GetAllSensorsWithLastValues())
                .Returns(mockTempdataList);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Dashboard", viewResult.ViewData["Title"]);
            Assert.Single(model.Sensors);
            Assert.Equal(float.NaN, model.Sensors.First().Data);
        }

        [Fact]
        public void Index_ReturnsAllSensorsWithValues()
        {
            // Arrange
            var mockSensors = new List<Sensor>
            {
                new Sensor { Name = "outdoor", Description = "zewnêtrzny", InternalId = "1", Type = SensorType.Temperature },
                new Sensor { Name = "indoor", Description = "wewnêtrzny", InternalId = "2", Type = SensorType.Temperature },
                new Sensor { Name = "indoor2", Description = "wewnêtrzny2", InternalId = "3", Type = SensorType.SoilHumidity }
            };

            mockSensors[0].Values = new List<SensorValue>
            {
                new SensorValue { Data = 1.0f, Sensor = mockSensors[0], Timestamp = DateTime.UtcNow },
                new SensorValue { Data = 2.0f, Sensor = mockSensors[0], Timestamp = DateTime.UtcNow - TimeSpan.FromMinutes(15) }
            };

            mockSensors[1].Values = new List<SensorValue>
            {
                new SensorValue { Data = 2.0f, Sensor = mockSensors[1], Timestamp = DateTime.UtcNow },
                new SensorValue { Data = 1.0f, Sensor = mockSensors[1], Timestamp = DateTime.UtcNow - TimeSpan.FromMinutes(15) }
            };

            mockSensors[2].Values = new List<SensorValue>
            {
                new SensorValue { Data = 1.0f, Sensor = mockSensors[1], Timestamp = DateTime.UtcNow },
                new SensorValue { Data = 1.2f, Sensor = mockSensors[1], Timestamp = DateTime.UtcNow - TimeSpan.FromMinutes(15) }
            };

            sensorsRepoMock
                .Setup(repo => repo.GetAllSensorsWithLastValues())
                .Returns(mockSensors);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Dashboard", viewResult.ViewData["Title"]);
            Assert.Equal(3, model.Sensors.Count());
            Assert.Equal(1.0f, model.Sensors.First().Data);
            Assert.Equal(Difference.Lowering, model.Sensors.First().Trend);
            Assert.Equal(Difference.Rising, model.Sensors.Skip(1).First().Trend);
            Assert.Equal(Difference.Steady, model.Sensors.Skip(2).First().Trend);
        }

        [Fact]
        public void Index_ReturnsSingleSensor()
        {
            var now = DateTime.Now;

            // Arrange
            var mockSensor = new Sensor
            {
                Name = "outdoor",
                Description = "zewn¹trz",
                InternalId = "1",
                Type = SensorType.Temperature,
                Values = new List<SensorValue>
                {
                    new SensorValue {  Data = 1, Id = 1, Timestamp = now },
                    new SensorValue {  Data = 2, Id = 2, Timestamp = now - TimeSpan.FromMinutes(15) },
                    new SensorValue {  Data = 3, Id = 3, Timestamp = now - TimeSpan.FromMinutes(30) }
                }
            };

            sensorsRepoMock
                .Setup(repo => repo.GetSensor("outdoor"))
                .Returns(mockSensor);

            sensorsRepoMock
                .Setup(repo => repo.GetSensorHistoryLast24Hours("outdoor"))
                .Returns(new List<GroupedByDateTime>() {
                    new GroupedByDateTime { Timestamp = now, Value = (1 + 2 + 3) / 3.0f }
                });

            // Act
            var result = controller.Sensor("outdoor");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var svm = Assert.IsAssignableFrom<ViewModels.SensorPageViewModel>(viewResult.ViewData.Model);

            Assert.True(svm.Sensor.Status);
            Assert.Equal("outdoor", svm.Sensor.Name);
            Assert.Equal("zewn¹trz", svm.Sensor.Description);
            Assert.Equal("1", svm.Sensor.InternalId);
            Assert.Equal(SensorType.Temperature, svm.Sensor.Type);
            Assert.Equal(1, svm.Sensor.Data);
            Assert.Equal(now, svm.Sensor.LastUpdated);
            Assert.Single(svm.Last24Hours);
            Assert.Equal((1 + 2 + 3) / 3.0f, svm.Last24Hours.First().Value);
        }
    }
}