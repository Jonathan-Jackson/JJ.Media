using JJ.Media.MediaInfo.API;
using MediaInfo.API.Client;
using MediaInfo.API.Client.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;

namespace MediaInfo.IntegrationTesting {

    public class TestBase {
        private const bool IsDebug = true;
        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Startup> _app;
        protected readonly IServiceProvider _services;
        protected readonly MediaInfoClient _apiClient;

        public TestBase() {
            _app = new WebApplicationFactory<Startup>();
            _services = _app.Server.Services;

            if (IsDebug) {
                _client = _app.CreateClient();
                _apiClient = new MediaInfoClient(_client, new MediaInfoClientOptions());
            }
            else {
                _client = new HttpClient { BaseAddress = new Uri("http://htpc:4681") };
                _apiClient = new MediaInfoClient(new HttpClient(), new MediaInfoClientOptions {
                    Address = "http://htpc:4681"
                });
            }
        }
    }
}