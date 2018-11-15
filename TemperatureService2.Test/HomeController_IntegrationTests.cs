using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace TemperatureService2.Test
{
    public class HomeController_Integration : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public HomeController_Integration(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Index")]
        [InlineData("/outdoor.html")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/nonexisting.html")]
        [InlineData("/nonexisting.json")]
        [InlineData("/nonexisting.wns")]
        public async Task Get_SensorsReturn404WhenNonExisting(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url).ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("/outdoor.json")]
        [InlineData("/indoor.json")]
        public async Task Get_SensorReturn200AndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/outdoor.wns")]
        [InlineData("/indoor.wns")]
        public async Task Get_SensorReturn200AndCorrectContentTypeAndCorrectWNSExpires(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(await response.Content.ReadAsStringAsync());

            var text = xd.SelectNodes("/tile/visual/binding[@template=\"TileWide\"]/text[@hint-style=\"captionSubtle\"]").Item(0).InnerText;
            text = text.Replace("last update: ", "");

            var lastUpdated = DateTime.Parse(text);
            var wnsExpires = DateTime.Parse(response.Headers.GetValues("X-WNS-Expires").First()).ToUniversalTime();

            Assert.Equal(lastUpdated.AddMinutes(60), wnsExpires);
            Assert.Equal("application/xml; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}