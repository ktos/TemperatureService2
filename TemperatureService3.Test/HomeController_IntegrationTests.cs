using Newtonsoft.Json;
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
            var response = await DoGet(url);

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
            var response = await DoGet(url);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("/outdoor.json")]
        [InlineData("/indoor.json")]
        public async Task Get_SensorReturn200AndCorrectContentType(string url)
        {
            var response = await DoGet(url);

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        private async Task<HttpResponseMessage> DoGet(string url)
        {
            var client = _factory.CreateClient();
            return await client.GetAsync(url).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("/outdoor.wns")]
        [InlineData("/indoor.wns")]
        public async Task Get_SensorReturn200AndCorrectContentTypeAndCorrectWNSExpires(string url)
        {
            var response = await DoGet(url);

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
            var response = await DoGet(url);

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
            var response = await DoGet(url);

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
            var response = await DoGet(url);

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
            var response = await DoGet(url);

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
            var response = await DoGet(url);

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
            var response = await DoGet(url);

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
            var url = $"/{sensor}";
            var dto = new SensorDto { ApiKey = "testapi", Data = 9 };
            var content = JsonConvert.SerializeObject(dto);

            var response = await DoPut(url, content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(9.0f, svm.Data);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        private async Task<HttpResponseMessage> DoPut(string url, string content, string apiKeyHeaderContent = null)
        {
            var client = _factory.CreateClient();

            if (apiKeyHeaderContent != null)
            {
                client.DefaultRequestHeaders.Add("X-APIKEY", apiKeyHeaderContent);
            }

            return await client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> DoPost(string url, string content, string apiKeyHeaderContent = null)
        {
            var client = _factory.CreateClient();

            if (apiKeyHeaderContent != null)
            {
                client.DefaultRequestHeaders.Add("X-APIKEY", apiKeyHeaderContent);
            }

            return await client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task Put_SensorDataWrongModelWithoutAuth_ShouldBe401()
        {
            var url = $"/outdoor";
            var content = "{aafgaaga";

            var response = await DoPut(url, content);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_SensorDataWrongModelAuthorized_ShouldBe400()
        {
            var url = $"/outdoor";
            var content = "{aafgaaga";
            var auth = "testapi";

            var response = await DoPut(url, content, auth);

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
            var auth = "testapi";

            var dto = new SensorDto { Data = 9 };
            var content = JsonConvert.SerializeObject(dto);

            var response = await DoPut(url, content, auth);

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
            var url = $"/{sensor}";

            var response = await DoGet($"/{sensor}.json");
            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            var oldData = svm.Data;

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription, ApiKey = "testapi" };
            var content = JsonConvert.SerializeObject(dto);

            response = await DoPut(url, content);

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
            var url = $"/{sensor}";

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);
            var auth = "testapi";

            var dto = new SensorDto { Id = randomId, Description = randomDescription };
            var content = JsonConvert.SerializeObject(dto);

            var response = await DoPut(url, content, auth);

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
            var url = $"/{sensor}";
            var auth = "testapi";

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);
            float randomData = Utilities.RandomFloat();

            var dto = new SensorDto { Id = randomId, Description = randomDescription, Data = randomData };
            var content = JsonConvert.SerializeObject(dto);

            var response = await DoPut(url, content, auth);

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
            var auth = "testapi";
            var url = $"/{sensor}";

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription };
            var content = JsonConvert.SerializeObject(dto);

            var response = await DoPut(url, content, auth);

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
        [InlineData("indoor")]
        [InlineData("soil")]
        public async Task Put_UpdateSomeSensorMetadata(string sensor)
        {
            var url = $"/{sensor}";
            var url2 = $"/{sensor}.json";
            var auth = "testapi";

            // get old data
            var response = await DoGet(url2);
            var old = JsonConvert.DeserializeObject<SensorDto>(await response.Content.ReadAsStringAsync());

            // update data
            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription };
            var content = JsonConvert.SerializeObject(dto);

            response = await DoPut(url, content, auth);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(randomId, svm.Id);
            Assert.Equal(randomDescription, svm.Description);
            Assert.Equal(old.Type, svm.Type);
            Assert.NotEqual(svm.Description, old.Description);
            Assert.NotEqual(svm.Id, old.Id);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("indoor")]
        [InlineData("soil")]
        public async Task Post_UpdateSomeSensorMetadata(string sensor)
        {
            var url = $"/{sensor}";
            var url2 = $"/{sensor}.json";
            var auth = "testapi";

            // get old data
            var response = await DoGet(url2);
            var old = JsonConvert.DeserializeObject<SensorDto>(await response.Content.ReadAsStringAsync());

            // update data
            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription };
            var content = JsonConvert.SerializeObject(dto);

            response = await DoPost(url, content, auth);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(randomId, svm.Id);
            Assert.Equal(randomDescription, svm.Description);
            Assert.Equal(old.Type, svm.Type);
            Assert.NotEqual(svm.Description, old.Description);
            Assert.NotEqual(svm.Id, old.Id);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("outdoor")]
        public async Task Put_UpdateSensorWithData127_ShouldBe400(string sensor)
        {
            var url = $"/{sensor}";
            var auth = "testapi";

            var dto = new SensorDto { Data = -127 };
            var content = JsonConvert.SerializeObject(dto);

            var response = await DoPut(url, content, auth);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Put_SensorInformationWrongApiKeyInBody_ShouldBe401(string sensor)
        {
            // Arrange
            var auth = "wrongapikey";
            var url = $"/{sensor}";

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription, ApiKey = auth };
            var content = JsonConvert.SerializeObject(dto);

            var response = await DoPut(url, content);

            // Assert
            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Put_SensorInformationWrongApiKeyInHeaders_ShouldBe401(string sensor)
        {
            // Arrange
            var auth = "wrongapikey";
            var url = $"/{sensor}";

            string randomId = Utilities.RandomString(5);
            string randomDescription = Utilities.RandomString(20);

            var dto = new SensorDto { Id = randomId, Description = randomDescription };
            var content = JsonConvert.SerializeObject(dto);

            var response = await DoPut(url, content, auth);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Put_SensorDataLikeDevice(string sensor)
        {
            // Arrange
            var auth = "testapi";
            var url = $"/{sensor}";

            var content = "{ \"name\": \"" + sensor + "\", \"data\": \"" + "9.0" + "\", \"apikey\": \"" + auth + "\" }";

            var response = await DoPut(url, content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(9.0f, svm.Data);
            Assert.NotNull(svm.Description);
            Assert.NotEqual(string.Empty, svm.Description);
            Assert.NotNull(svm.Id);
            Assert.NotEqual(string.Empty, svm.Id);

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("outdoor")]
        [InlineData("indoor")]
        public async Task Post_SensorDataLikeDeviceAndCheckMetadata(string sensor)
        {
            // Arrange
            var auth = "testapi";
            var url = $"/{sensor}";
            var data = Utilities.RandomFloat().ToString("F2", CultureInfo.InvariantCulture);

            var content = "{ \"name\": \"" + sensor + "\", \"data\": \"" + data + "\", \"apikey\": \"" + auth + "\" }";

            var now = DateTime.UtcNow;

            var response = await DoPost(url, content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseBody = await response.Content.ReadAsStringAsync();
            var svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(data, svm.Data.ToString("F2", CultureInfo.InvariantCulture));

            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            response = await DoGet($"/{sensor}.json");
            responseBody = await response.Content.ReadAsStringAsync();
            svm = JsonConvert.DeserializeObject<SensorDto>(responseBody);

            Assert.Equal(data, svm.Data.ToString("F2", CultureInfo.InvariantCulture));

            var timeDiff = svm.LastUpdated.ToUniversalTime() - now;

            Assert.InRange(timeDiff.TotalMilliseconds, -10000, 10000);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            Assert.NotNull(svm.Description);
            Assert.NotEqual(string.Empty, svm.Description);
            Assert.NotNull(svm.Id);
            Assert.NotEqual(string.Empty, svm.Id);
        }

        [Fact]
        public async Task Get_HealthEndpointsReturn200AndJson()
        {
            var response = await DoGet("/health");

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());
        }
    }
}