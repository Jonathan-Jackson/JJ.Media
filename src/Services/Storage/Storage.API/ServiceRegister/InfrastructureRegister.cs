using Converter.API.Client;
using Converter.API.Client.Client;
using JJ.Framework.Repository;
using JJ.Framework.Repository.Abstraction;
using MediaInfo.API.Client;
using MediaInfo.API.Client.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Compilers;
using Storage.Domain.Helpers.Repository;
using Storage.Infrastructure.Repositories;
using System;
using System.Net.Http;
using System.Text.Json;

namespace Storage.API.ServiceRegister {

    public static class InfrastructureRegister {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
            services
                .AddTransient<MediaInfoClient>()
                .AddTransient<ConverterClient>()
                .AddTransient<IProcessedEpisodeRepository, ProcessedEpisodesRepository>()
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddSingleton<Compiler>(x => new SqlServerCompiler())
                .AddSingleton<HttpClient>();

            // Add Config Options.
            var convertOptions = configuration.GetSection("ConverterOptions").Get<ConverterClientOptions>();
            var mediaInfoOptions = configuration.GetSection("MediaInfoOptions").Get<MediaInfoClientOptions>();
            var storageFactoryConnString = configuration.GetConnectionString("StorageFactory");

            if (string.IsNullOrWhiteSpace(storageFactoryConnString))
                storageFactoryConnString = Environment.GetEnvironmentVariable("StorageFactory_DB", EnvironmentVariableTarget.User)
                                                ?? throw new ApplicationException("StorageFactory_DB Database Connection value is missing.");

            return services
                .AddSingleton(mediaInfoOptions)
                .AddSingleton(convertOptions)
                .AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(storageFactoryConnString))
                .AddSingleton(_ => new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}