using Microsoft.AspNetCore.Mvc.Testing;
using Storage.API;
using System;
using System.Net.Http;

namespace Storage.IntegrationTesting {

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