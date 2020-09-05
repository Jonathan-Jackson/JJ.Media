using JJ.Framework.Helpers;
using JJ.Framework.Helpers.Options;
using JJ.Framework.Repository;
using JJ.Framework.Repository.Abstraction;
using JJ.Framework.Repository.RabbitMq;
using MediaInfo.API.Client;
using MediaInfo.API.Client.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlKata.Compilers;
using Storage.Domain.DomainLayer.Processor;
using Storage.Domain.DomainLayer.Store;
using Storage.Domain.Helpers.Abstraction;
using Storage.Domain.Helpers.DTOs;
using Storage.Domain.Helpers.Options;
using Storage.Domain.Helpers.Repository;
using Storage.Domain.Repositories;
using Storage.Domain.Services;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Storage.Core.ServiceRegister {

    public static class DependencyModule {

        public static IServiceCollection AddDefaultServices(this IServiceCollection services, IConfiguration config) {
            string dbConnString = EnviromentHelper.FindGlobalEnviromentVariable("STORAGE_DB")
                    ?? (!string.IsNullOrWhiteSpace(config.GetConnectionString("StorageFactory"))
                        ? config.GetConnectionString("StorageFactory")
                        : throw new ApplicationException("STORAGE_DB NOT SPECIFIED. USE AN ENVIROMENT VAR."));

            return services
                // Services
                .AddSingleton<EpisodeProcessor>()
                .AddSingleton<IShowStore, ShowPhysicalStore>()
                .AddSingleton<IEpisodeStore, EpisodePhysicalStore>()
                .AddSingleton<StorageService>()
                .AddSingleton<EpisodeProcessor>()
                // Config
                .AddLogging(configure => configure.AddConsole())
                .AddSingleton(BuildMediaStorageOptions(config))
                .AddSingleton(BuildBrokerOptions(config))
                .AddSingleton(BuildMediaInfoClientOptions(config))
                // Infrastructure
                .AddSingleton<IMessageBroker, RabbitBroker>(provider => {
                    var options = provider.GetRequiredService<BrokerOptions>();
                    return new RabbitBroker(options.Address, options.UserName, options.Password, provider.GetRequiredService<ILogger<RabbitBroker>>());
                })
                .AddSingleton<IMediaInfoClient, MediaInfoClient>()
                .AddSingleton<IProcessedEpisodeRepository, ProcessedEpisodesRepository>()
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddSingleton<Compiler>(x => new SqlServerCompiler())
                .AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(dbConnString))
                .AddSingleton<HttpClient>();
        }

        private static BrokerOptions BuildBrokerOptions(IConfiguration config) {
            // Get.
            var options = config.GetSection("BrokerOptions").Get<BrokerOptions>();
            options.Address = EnviromentHelper.GetSetting("BROKER_ADDRESS", options.Address);
            options.UserName = EnviromentHelper.GetSetting("BROKER_USERNAME", options.UserName, allowEmpty: true);
            options.Password = EnviromentHelper.GetSetting("BROKER_PASSWORD", options.Password, allowEmpty: true);

            // Validate.
            if (!RabbitMqHelper.TryConnect(options.Address, options.UserName, options.Password, out Exception ex))
                throw new ApplicationException($"Could not connect to RabbitMq Server on the hostname: {options.Address} ({ex.Message})");

            return options;
        }

        private static MediaInfoClientOptions BuildMediaInfoClientOptions(IConfiguration config) {
            // Get.
            var options = config.GetSection("MediaInfoOptions").Get<MediaInfoClientOptions>();
            options.Address = EnviromentHelper.GetSetting("STORAGE_MEDIAINFOADDRESS", options.Address);

            return options;
        }

        private static MediaStorageOptions BuildMediaStorageOptions(IConfiguration config) {
            // Get.
            var options = config.GetSection("MediaStorageOptions").Get<MediaStorageOptions>();
            options.Stores = EnviromentHelper.FindGlobalEnviromentJsonVariable<StoreArea[]>("STORAGE_STORES")
                    ?? options.Stores;
            options.ProcessStores = EnviromentHelper.FindGlobalEnviromentJsonVariable<StoreArea[]>("STORAGE_PROCESSSTORES")
                    ?? options.ProcessStores;

            // Validate.
            if (options?.Stores?.Any() != true)
                throw new ApplicationException("There are no storage option stores found.");
            var missingStores = options.Stores.Where(dir => !Directory.Exists(dir.Path));
            if (missingStores.Any())
                throw new ApplicationException($"Cannot find storage option stores: {string.Join(", ", missingStores)}");

            if (options?.ProcessStores?.Any() != true)
                throw new ApplicationException("There are no storage option -processing- stores found.");
            missingStores = options.ProcessStores.Where(dir => !Directory.Exists(dir.Path));
            if (missingStores.Any())
                throw new ApplicationException($"Cannot find storage option -processing- stores: {string.Join(", ", missingStores)}");

            return options;
        }
    }
}