using JJ.Media.MediaInfo.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;

namespace MediaInfo.IntegrationTesting {

    public class TestBase {
        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Startup> _app;
        protected readonly IServiceProvider _services;

        public TestBase() {
            _app = new WebApplicationFactory<Startup>();
            _client = _app.CreateClient();
            _services = _app.Server.Services;
        }
    }
}