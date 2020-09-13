using Downloader.Core.Feeds;
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

        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration, ILogger logger) {
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
            torrentOptions.DownloadTorrentPath = EnviromentHelper.GetLoggedSetting(logger, "DOWNLOAD_PATH", torrentOptions.DownloadTorrentPath);

            if (!Directory.Exists(torrentOptions.DownloadTorrentPath))
                throw new ApplicationException($"Download Directory does not exist: {torrentOptions.DownloadTorrentPath} (FULL PATH: {Path.GetFullPath(torrentOptions.DownloadTorrentPath)}");

            // HorribleSubs options.
            var horribleOptions = configuration.GetSection("HorribleSubsOptions").Get<HorribleSubsOptions>();
            horribleOptions.Quality = EnviromentHelper.GetLoggedSetting(logger, "HORRIBLESUBS_QUALITY", horribleOptions.Quality);

            // QBitTorrent Options
            var qbitOptions = configuration.GetSection("QBitOptions").Get<QBitOptions>();
            qbitOptions.Address = EnviromentHelper.GetLoggedSetting(logger, "QBITTORRENT_ADDRESS", qbitOptions.Address);
            qbitOptions.UserName = EnviromentHelper.GetLoggedSetting(logger, "QBITTORRENT_USERNAME", qbitOptions.UserName);
            qbitOptions.Password = EnviromentHelper.GetLoggedSetting(logger, "QBITTORRENT_PASSWORD", qbitOptions.Password);

            // Broker Options
            var brokerOptions = configuration.GetSection("BrokerOptions").Get<BrokerOptions>();
            brokerOptions.Address = EnviromentHelper.GetLoggedSetting(logger, "BROKER_ADDRESS", brokerOptions.Address);
            brokerOptions.UserName = EnviromentHelper.GetLoggedSetting(logger, "BROKER_USERNAME", brokerOptions.UserName, allowEmpty: true);
            brokerOptions.Password = EnviromentHelper.GetLoggedSetting(logger, "BROKER_PASSWORD", brokerOptions.Password, allowEmpty: true);

            if (!RabbitMqHelper.TryConnect(brokerOptions.Address, brokerOptions.UserName, brokerOptions.Password, out Exception ex))
                throw new ApplicationException($"Could not connect to RabbitMq Server on the hostname: {brokerOptions.Address} ({ex.Message})");

            // DB String
            var downloaderConnString = EnviromentHelper.GetLoggedSetting(logger, "DOWNLOADFACTORY_DB", configuration.GetConnectionString("DownloaderFactory"));

            services
                .AddSingleton(torrentOptions)
                .AddSingleton(horribleOptions)
                .AddSingleton(qbitOptions)
                .AddSingleton(brokerOptions)
                .AddSingleton<IMessageBroker, RabbitBroker>(provider => {
                    var options = provider.GetRequiredService<BrokerOptions>();
                    return new RabbitBroker(options.Address, options.UserName, options.Password, provider.GetRequiredService<ILogger<RabbitBroker>>());
                })
                .AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(downloaderConnString));

            return services;
        }
    }
}