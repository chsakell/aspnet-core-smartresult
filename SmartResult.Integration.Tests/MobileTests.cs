using AspNet.Core.SmartResult.Demo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using SmartResult.Demo.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SmartResult.Integration.Tests
{
    public class MobileTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public MobileTests()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task Returns_Mobile_Result_When_Android_In_Header()
        {
            // Act
            _client.DefaultRequestHeaders.Add("User-Agent", "android");
            var response = await _client.GetAsync("/api/customers/");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var headerResult = response.Headers.GetValues("Result-Type").ToList().First();
            var responseResult = JsonConvert.DeserializeObject<List<MobileCustomer>>(responseString);
            // Assert
            Assert.Equal("Mobile", headerResult);
            Assert.IsType<List<MobileCustomer>>(responseResult);
        }
    }
}
