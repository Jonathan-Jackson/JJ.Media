﻿using Downloader.Core.Feeds;
using Downloader.Core.Helpers;
using Downloader.Core.Helpers.Options;
using Downloader.Core.Infrastructure;
using Downloader.Core.Services;
using JJ.Framework.Helpers;
using JJ.Framework.Helpers.Options;
using JJ.Framework.Repository;
using JJ.Framework.Repository.Abstraction;
using JJ.Framework.Repository.RabbitMq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using System.IO;
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
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddSingleton<Compiler>(x => new SqlServerCompiler())
                .AddSingleton<HttpClient>()
                // Config
                .AddLogging(configure => configure.AddConsole());

            // Add Config Options.
            var torrentOptions = configuration.GetSection("TorrentServiceOptions").Get<TorrentServiceOptions>();
            torrentOptions.DownloadTorrentPath = EnviromentHelper.GetSetting("DOWNLOAD_PATH", torrentOptions.DownloadTorrentPath);

            if (!Directory.Exists(torrentOptions.DownloadTorrentPath))
                throw new ApplicationException($"Download Directory does not exist: {torrentOptions.DownloadTorrentPath} (FULL PATH: {Path.GetFullPath(torrentOptions.DownloadTorrentPath)}");

            // HorribleSubs options.
            var horribleOptions = configuration.GetSection("HorribleSubsOptions").Get<HorribleSubsOptions>();
            horribleOptions.Quality = EnviromentHelper.GetSetting("HORRIBLESUBS_QUALITY", horribleOptions.Quality);

            // QBitTorrent Options
            var qbitOptions = configuration.GetSection("QBitOptions").Get<QBitOptions>();
            qbitOptions.Address = EnviromentHelper.GetSetting("QBITTORRENT_ADDRESS", qbitOptions.Address);

            // Broker Options
            var brokerOptions = configuration.GetSection("BrokerOptions").Get<BrokerOptions>();
            brokerOptions.Address = EnviromentHelper.GetSetting("BROKER_ADDRESS", brokerOptions.Address);

            // DB String
            var downloaderConnString = EnviromentHelper.GetSetting("DOWNLOADFACTORY_DB", configuration.GetConnectionString("DownloaderFactory"));

            return services
                .AddSingleton(torrentOptions)
                .AddSingleton(horribleOptions)
                .AddSingleton(qbitOptions)
                .AddSingleton(brokerOptions)
                .AddSingleton<IMessageBroker, RabbitBroker>(provider => new RabbitBroker(provider.GetRequiredService<BrokerOptions>().Address, provider.GetRequiredService<ILogger<RabbitBroker>>()))
                .AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(downloaderConnString));
        }
    }
}