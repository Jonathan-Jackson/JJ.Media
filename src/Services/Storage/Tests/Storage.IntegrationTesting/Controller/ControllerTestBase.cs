using Microsoft.AspNetCore.Mvc.Testing;
using Storage.API;
using System;
using System.Net.Http;

namespace Storage.IntegrationTesting.Controller {

    public class ControllerTestBase {
        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Startup> _app;
        protected readonly IServiceProvider _services;

        public ControllerTestBase() {
            _app = new WebApplicationFactory<Startup>();
            _client = _app.CreateClient();
            _services = _app.Server.Services;
        }
    }
}