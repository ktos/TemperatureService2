using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperatureService2.Controllers;
using TemperatureService2.Models;
using TemperatureService2.Repository;
using Xunit;

namespace TemperatureService2.Test
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
                new Sensor { Name = "indoor", Description = "wewnêtrzny", InternalId = "2", Type = SensorType.Temperature }
            };

            sensorsRepoMock
                .Setup(repo => repo.GetAllSensors())
                .Returns(mockTempdataList);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Sensor>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
            Assert.Null(model.First().Values);
        }

        [Fact]
        public void Index_ReturnsSingleSensor()
        {
            var now = DateTime.UtcNow;

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

            // Act
            var result = controller.Sensor("outdoor");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var svm = Assert.IsAssignableFrom<ViewModels.SensorViewModel>(viewResult.ViewData.Model);

            Assert.True(svm.Status);
            Assert.Equal("outdoor", svm.Name);
            Assert.Equal("zewn¹trz", svm.Description);
            Assert.Equal("1", svm.Id);
            Assert.Equal(SensorType.Temperature, svm.Type);
            Assert.Equal(1, svm.Data);
            Assert.Equal(now, svm.LastUpdated);
        }
    }
}
