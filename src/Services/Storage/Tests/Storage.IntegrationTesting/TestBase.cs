using Microsoft.AspNetCore.Mvc.Testing;
using Storage.API;
using Storage.API.Client;
using Storage.API.Client.Client;
using System;
using System.Net.Http;

namespace Storage.IntegrationTesting {

    public class TestBase {
        private const bool IsDebug = false;

        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Startup> _app;
        protected readonly IServiceProvider _services;
        protected readonly StorageClient _apiClient;

        public TestBase() {
            _app = new WebApplicationFactory<Startup>();
            _services = _app.Server.Services;

            if (IsDebug) {
                _client = _app.CreateClient();
                _apiClient = new StorageClient(_client, new StorageClientOptions());
            }
            else {
                _client = new HttpClient { BaseAddress = new Uri("http://htpc:4682") };
                _apiClient = new StorageClient(new HttpClient(), new StorageClientOptions {
                    Address = "http://htpc:4682"
                });
            }
        }
    }
}