using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Helpful.Hosting.WindowsService.Core.IntegrationTests
{
    public class EndpointTests
    {
        private HttpClient _httpClient;
        private static int[] _ports = {8050, 8051, 8052, 8053};
        private IConfigurationRoot _config;

        [OneTimeSetUp]
        public void Setup()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();
            _httpClient = new HttpClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }

        [Test]
        public async Task HealthCheckIsGreen([ValueSource(nameof(_ports))] int port)
        {
            var uri = $"{_config.GetSection("environmentSpecificSettings")["baseUrl"]}:{port}/healthcheck";
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri, UriKind.Absolute)
            };
            var response = await _httpClient.SendAsync(req);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task SwaggerEndpointResponds([ValueSource(nameof(_ports))] int port)
        {
            var uri = $"{_config.GetSection("environmentSpecificSettings")["baseUrl"]}:{port}/swagger";
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri, UriKind.Absolute)
            };
            var response = await _httpClient.SendAsync(req);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }
    }
}