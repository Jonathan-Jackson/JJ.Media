using Discord.API;
using Discord.API.Client;
using Discord.API.Client.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;

namespace Discord.IntegrationTesting {

    public class TestBase {
        private const bool IsDebug = false; // Switch to ENV?

        protected readonly DiscordClient _apiClient;
        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Startup> _app;
        protected readonly IServiceProvider _services;

        public TestBase() {
            _app = new WebApplicationFactory<Startup>();

            if (IsDebug) {
                _client = _app.CreateClient();
                _apiClient = new DiscordClient(_client, new DiscordClientOptions());
            }
            else {
                _client = new HttpClient { BaseAddress = new Uri("http://localhost:3432") };
                _apiClient = new DiscordClient(_client, new DiscordClientOptions { Address = "http://localhost:3432" });
            }

            _services = _app.Server.Services;
        }
    }
}