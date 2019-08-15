﻿using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TemperatureService3.Models;
using TemperatureService3.Test.Helpers;
using Xunit;

namespace TemperatureService3.Test
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
        [InlineData("/outdoor")]
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
        [InlineData("/nonexisting")]
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
            var value = xd.SelectNodes("/tile/visual/binding[@template=\"TileWide\"]/text[@hint-style=\"subtitle\"]").Item(0).InnerText;

            var lastUpdated = DateTime.Parse(text).ToUniversalTime();
            var wnsExpires = DateTime.Parse(response.Headers.GetValues("X-WNS-Expires").First()).ToUniversalTime();

            Assert.Contains("°C", value);
            Assert.Equal(lastUpdated.AddMinutes(60), wnsExpires);
            Assert.Equal("application/xml; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/soil.wns")]
        public async Task Get_SensorReturn200AndCorrectWNSFormat(string url)
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
            var value = xd.SelectNodes("/tile/visual/binding[@template=\"TileWide\"]/text[@hint-style=\"subtitle\"]").Item(0).InnerText;

            text = text.Replace("last update: ", "");

            var lastUpdated = DateTime.Parse(text).ToUniversalTime();
            var wnsExpires = DateTime.Parse(response.Headers.GetValues("X-WNS-Expires").First()).ToUniversalTime();

            Assert.DoesNotContain("°C", value);
            Assert.Equal(lastUpdated.AddMinutes(60), wnsExpires);
            Assert.Equal("application/xml; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/outdoor.html")]
        public async Task Get_SingleSensorHistoryLastWeek(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();

            var lastJson = ExtractJson(resp, "lastweek");
            var lasthistory = JsonConvert.DeserializeObject<LabelsData>(lastJson);

            Assert.Equal(2, lasthistory.labels.Length);
            Assert.Equal(2, lasthistory.data.Length);
            Assert.InRange(lasthistory.data[0], 3.0f, 3.5f);
            Assert.InRange(lasthistory.data[1], 3.0f, 3.5f);
            Assert.Equal(DateTime.Now.AddDays(-1).DayOfYear, DateTime.Parse(lasthistory.labels[0]).DayOfYear);
            Assert.Equal(DateTime.Now.DayOfYear, DateTime.Parse(lasthistory.labels[1]).DayOfYear);
        }

        [Theory]
        [InlineData("/outdoor.html")]
        public async Task Get_SingleSensorHistoryLastMonth(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();

            var lastJson = ExtractJson(resp, "lastmonth");
            var lasthistory = JsonConvert.DeserializeObject<LabelsData>(lastJson);

            Assert.Equal(2, lasthistory.labels.Length);
            Assert.Equal(2, lasthistory.data.Length);
            Assert.InRange(lasthistory.data[0], 3.0f, 3.5f);
            Assert.InRange(lasthistory.data[1], 3.0f, 3.5f);
            Assert.Equal(DateTime.Now.AddDays(-1).DayOfYear, DateTime.Parse(lasthistory.labels[0]).DayOfYear);
            Assert.Equal(DateTime.Now.DayOfYear, DateTime.Parse(lasthistory.labels[1]).DayOfYear);
        }

        [Theory]
        [InlineData("/outdoor.html")]
        public async Task Get_SingleSensorHistoryLastYear2(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();

            var lastJson = ExtractJson(resp, "lastyear2");
            var lasthistory = JsonConvert.DeserializeObject<LabelsData>(lastJson);

            Assert.Equal(2, lasthistory.labels.Length);
            Assert.Equal(2, lasthistory.data.Length);
            Assert.InRange(lasthistory.data[0], 3.0f, 3.5f);
            Assert.InRange(lasthistory.data[1], 3.0f, 3.5f);
            Assert.Equal(DateTime.Now.AddDays(-1).DayOfYear, DateTime.Parse(lasthistory.labels[0]).DayOfYear);
            Assert.Equal(DateTime.Now.DayOfYear, DateTime.Parse(lasthistory.labels[1]).DayOfYear);
        }

        [Theory]
        [InlineData("/outdoor.html")]
        public async Task Get_SingleSensorHistoryLastYear(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();

            var lastJson = ExtractJson(resp, "lastyear");
            var lasthistory = JsonConvert.DeserializeObject<LabelsData>(lastJson);

            Assert.Single(lasthistory.labels);
            Assert.Single(lasthistory.data);
            Assert.InRange(lasthistory.data[0], 3.0f, 3.5f);
        }

        [Theory]
        [InlineData("/outdoor.html")]
        public async Task Get_SingleSensorHistoryLast24(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();

            var lastJson = ExtractJson(resp, "last24");
            var lasthistory = JsonConvert.DeserializeObject<LabelsData>(lastJson);

            Assert.InRange(lasthistory.labels.Length, 23, 26);
            Assert.InRange(lasthistory.data.Length, 23, 26);
        }

        private string ExtractJson(string data, string objstart)
        {
            var search = $"let {objstart} = {{";
            var start = data.IndexOf(search);
            if (start == -1)
                return string.Empty;

            var stop = data.IndexOf("};", start);

            var result = data.Substring(start, stop - start);

            return result.Substring(search.Length - 1) + "}";
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Put_SensorDataApiKeyInBody(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";

            var dto = new SensorDto { ApiKey = "testapi", Data = 9 };
            var content = JsonConvert.SerializeObject(dto);

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(9.0f, svm.Data);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Put_SensorDataWrongModel()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/outdoor";

            var content = "{aafgaaga";

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_SensorDataWrongModelAuthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/outdoor";

            var content = "{aafgaaga";
            client.DefaultRequestHeaders.Add("X-APIKEY", "testapi");

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Put_SensorDataApiKeyInHeaders(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";

            var dto = new SensorDto { Data = 9 };
            var content = JsonConvert.SerializeObject(dto);

            client.DefaultRequestHeaders.Add("X-APIKEY", "testapi");

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(9.0f, svm.Data);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Put_SensorInformationApiKeyInBody(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";

            var response = await client.GetAsync($"/{sensor}.json").ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            var oldData = svm.Data;

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription, ApiKey = "testapi" };
            var content = JsonConvert.SerializeObject(dto);

            // Act
            response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            responseBody = await response.Content.ReadAsStringAsync();
            svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(oldData, svm.Data);
            Assert.Equal(randomId, svm.Id);
            Assert.Equal(randomDescription, svm.Description);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Put_SensorInformationApiKeyInHeaders(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription };
            var content = JsonConvert.SerializeObject(dto);

            client.DefaultRequestHeaders.Add("X-APIKEY", "testapi");

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(randomId, svm.Id);
            Assert.Equal(randomDescription, svm.Description);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("newsensor")]
        public async Task Put_CreateNewSensorWithData(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);
            float randomData = Utilities.RandomFloat();

            var dto = new SensorDto { Id = randomId, Description = randomDescription, Data = randomData };
            var content = JsonConvert.SerializeObject(dto);

            client.DefaultRequestHeaders.Add("X-APIKEY", "testapi");

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(randomId, svm.Id);
            Assert.Equal(randomDescription, svm.Description);
            Assert.Equal(randomData, svm.Data);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("newsensor")]
        public async Task Put_CreateNewSensorWithoutData(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription };
            var content = JsonConvert.SerializeObject(dto);

            client.DefaultRequestHeaders.Add("X-APIKEY", "testapi");

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(randomId, svm.Id);
            Assert.Equal(randomDescription, svm.Description);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("outdoor")]
        public async Task Put_UpdateSensorWithData127(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";

            var dto = new SensorDto { Data = -127 };
            var content = JsonConvert.SerializeObject(dto);

            client.DefaultRequestHeaders.Add("X-APIKEY", "testapi");

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Put_SensorInformationWrongApiKeyInBody(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription, ApiKey = "wrongapikey" };
            var content = JsonConvert.SerializeObject(dto);

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            // Assert
            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Put_SensorInformationWrongApiKeyInHeaders(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription };
            var content = JsonConvert.SerializeObject(dto);

            client.DefaultRequestHeaders.Add("X-APIKEY", "wrongapikey");

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Put_SensorDataLikeDevice(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";

            var content = "{ \"name\": \"" + sensor + "\", \"data\": \"" + "9.0" + "\", \"apikey\": \"" + "testapi" + "\" }";

            // Act
            var response = await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(9.0f, svm.Data);
            Assert.NotNull(svm.Description);
            Assert.NotEqual(string.Empty, svm.Description);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Post_SensorDataLikeDevice(string sensor)
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = $"/{sensor}";
            var data = Utilities.RandomFloat().ToString("F2", CultureInfo.InvariantCulture);

            var content = "{ \"name\": \"" + sensor + "\", \"data\": \"" + data + "\", \"apikey\": \"" + "testapi" + "\" }";

            var now = DateTime.UtcNow;
            // Act
            var response = await client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(data, svm.Data.ToString("F2", CultureInfo.InvariantCulture));

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            response = await client.GetAsync($"/{sensor}.json").ConfigureAwait(false);
            responseBody = await response.Content.ReadAsStringAsync();
            svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(data, svm.Data.ToString("F2", CultureInfo.InvariantCulture));

            var timeDiff = svm.LastUpdated.ToUniversalTime() - now;

            Assert.InRange(timeDiff.TotalMilliseconds, -1000, 1000);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            Assert.NotNull(svm.Description);
            Assert.NotEqual(string.Empty, svm.Description);
        }
    }
}