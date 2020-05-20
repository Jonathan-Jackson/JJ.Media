using Downloader.Core.Feeds;
using Downloader.Core.Helpers;
using Downloader.Core.Helpers.Options;
using Downloader.Core.Infrastructure;
using Downloader.Core.Services;
using JJ.Framework.Repository;
using JJ.Framework.Repository.Abstraction;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlKata.Compilers;
using Storage.API.Client;
using Storage.API.Client.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Downloader.Core.ServiceRegister {

    public static class DefaultDependencies {

        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration) {
            services
                // Services
                .AddTransient<ITorrentClient, QBitService>()
                .AddTransient<DownloaderService>()
                .AddTransient<TorrentService>()
                .AddTransient<HorribleSubsFeed>()
                .AddTransient<IReadOnlyCollection<IFeed>>(builder => new[] { builder.GetRequiredService<HorribleSubsFeed>() })
                // Repos
                .AddTransient<HistoryRepository>()
                .AddTransient<StorageClient>()
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddSingleton<Compiler>(x => new SqlServerCompiler())
                .AddSingleton<HttpClient>()
                .AddLogging(configure => configure.AddConsole());

            // Add Config Options.
            var storageOptions = configuration.GetSection("StorageServiceOptions").Get<StorageClientOptions>();
            var torrentOptions = configuration.GetSection("TorrentServiceOptions").Get<TorrentServiceOptions>();
            var horribleOptions = configuration.GetSection("HorribleSubsOptions").Get<HorribleSubsOptions>();
            var qbitOptions = configuration.GetSection("QBitOptions").Get<QBitOptions>();

            var downloaderConnString = configuration.GetConnectionString("DownloaderFactory");
            if (string.IsNullOrWhiteSpace(downloaderConnString))
                downloaderConnString = Environment.GetEnvironmentVariable("DownloaderFactory_DB", EnvironmentVariableTarget.User) ?? throw new ApplicationException("DownloaderFactory_DB Database Connection value is missing.");

            return services
                .AddSingleton(storageOptions)
                .AddSingleton(torrentOptions)
                .AddSingleton(horribleOptions)
                .AddSingleton(qbitOptions)
                .AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(downloaderConnString));
        }
    }
}