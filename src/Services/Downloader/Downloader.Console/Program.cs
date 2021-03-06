﻿using Downloader.Core.ServiceRegister;
using Downloader.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Downloader.ConsoleUI {

    internal class Program {

        private static async Task Main(string[] args) {
            // Disable SSL ~ Services are ran locally, and for whatever
            // reason Kestral redirects to HTTPS even when specifying HTTP.
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            IServiceCollection services = new ServiceCollection();

            var provider = services.AddDependencies(config, CreateTempLogger()).BuildServiceProvider();

            await provider.GetRequiredService<DownloaderService>()
                            .Run();
        }

        private static ILogger CreateTempLogger() {
            // Log Settings
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });
            return loggerFactory.CreateLogger<Program>();
        }
    }
}