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
        private static (string, int)[] _httpInfo = {("http", 8050), ("https", 8051), ("http", 8052), ("http", 8053)};
        private IConfigurationRoot _config;

        [OneTimeSetUp]
        public void Setup()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();
            _httpClient = new HttpClient();

            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }

        [Test]
        public async Task HealthCheckIsGreen([ValueSource(nameof(_httpInfo))] (string proto, int port) httpInfo)
        {
            var uri = $"{httpInfo.proto}://{_config.GetSection("environmentSpecificSettings")["hostName"]}:{httpInfo.port}/healthcheck";
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri, UriKind.Absolute)
            };
            var response = await _httpClient.SendAsync(req);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task SwaggerEndpointResponds([ValueSource(nameof(_httpInfo))] (string proto, int port) httpInfo)
        {
            var uri = $"{httpInfo.proto}://{_config.GetSection("environmentSpecificSettings")["hostName"]}:{httpInfo.port}/swagger";
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