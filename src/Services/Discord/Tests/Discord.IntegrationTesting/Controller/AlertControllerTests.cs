using Discord.API.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Discord.IntegrationTesting.Controller {

    public class AlertControllerTests : TestBase {

        [Fact]
        public async Task Episode_OK() {
            var response = await _client.PostAsync($"/api/alert/episode", new StringContent(6.ToString(), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}