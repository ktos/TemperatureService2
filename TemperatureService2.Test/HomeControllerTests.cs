using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempHistory.Controllers;
using TempHistory.Models;
using TempHistory.Repository;
using Xunit;

namespace TemperatureService2.Test
{
    public class HomeControllerTests
    {
        private Mock<ITempdataRepository> articlesRepoMock;
        private HomeController controller;

        public HomeControllerTests()
        {
            articlesRepoMock = new Mock<ITempdataRepository>();
            controller = new HomeController(articlesRepoMock.Object);
        }

        [Fact]
        public void Index_ReturnsLast100OutdoorData()
        {
            // Arrange
            var mockTempdataList = new List<Tempdata>
            {
                new Tempdata { Sensor = "outdoor", Timestamp = 0, Value = 0 },
                new Tempdata { Sensor = "outdoor", Timestamp = 1, Value = 1 }
            };

            articlesRepoMock
                .Setup(repo => repo.GetLast100OutdoorData())
                .Returns(mockTempdataList);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Tempdata>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }
    }
}
