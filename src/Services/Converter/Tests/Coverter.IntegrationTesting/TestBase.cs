using Converter.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;

namespace Converter.IntegrationTesting {

    public class TestBase {
        private const bool IsDebug = true;

        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Startup> _app;
        protected readonly IServiceProvider _services;

        public TestBase() {
            _app = new WebApplicationFactory<Startup>();

            if (IsDebug)
                _client = _app.CreateClient();
            else
                _client = new HttpClient { BaseAddress = new Uri("http://htpc:5231") };

            _services = _app.Server.Services;
        }
    }
}