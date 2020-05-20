using Discord.API;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;

namespace Discord.IntegrationTesting {

    public class TestBase {
        private const bool IsDebug = true; // Switch to ENV?

        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Startup> _app;
        protected readonly IServiceProvider _services;

        public TestBase() {
            _app = new WebApplicationFactory<Startup>();

            if (IsDebug)
                _client = _app.CreateClient();
            else
                _client = new HttpClient { BaseAddress = new Uri("http://htpc:3432") };

            _services = _app.Server.Services;
        }
    }
}