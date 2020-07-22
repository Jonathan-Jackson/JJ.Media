using JJ.Framework.Helpers;
using JJ.Framework.Repository;
using JJ.Framework.Repository.Abstraction;
using JJ.Media.MediaInfo.API;
using JJ.Media.MediaInfo.Services.Interfaces;
using MediaInfo.Domain.Helpers.Repository;
using MediaInfo.Domain.Helpers.Repository.Interfaces;
using MediaInfo.Infrastructure.Client;
using MediaInfo.Infrastructure.Options;
using MediaInfo.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlKata.Compilers;
using System;
using TvDbSharper;

namespace MediaInfo.API.ServiceRegister {

    public static class InfrastructureRegister {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, ILogger? logger = null) {
            services
                // Helpers.
                .AddSingleton<ITvDbClient, TvDbClient>()
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddSingleton<Compiler>(x => new SqlServerCompiler())
                // Repos
                .AddTransient<IShowRepository, ShowRepository>()
                .AddTransient<IEpisodeRepository, EpisodeRepository>()
                // API
                .AddTransient<IApiSearch, TvDbSearch>()
                .AddTransient<IEpisodeApiSearch, TvDbSearch>()
                .AddTransient<IShowApiSearch, TvDbSearch>();

            // Add Config. Prioritize ENV over APPSettings.
            // TVDB Options
            var tvDbOptions = configuration.GetSection("TvDb").Get<TvDbOptions>();
            tvDbOptions.ApiKey = EnviromentHelper.FindGlobalEnviromentVariable("TVDB_APIKEY")
                ?? (!string.IsNullOrWhiteSpace(tvDbOptions.ApiKey) ? tvDbOptions.ApiKey : throw new ApplicationException("TVDB_APIKEY NOT SPECIFIED. USE AN ENVIROMENT VAR."));
            // MediaInfo Options
            var mediaInfoConnString = EnviromentHelper.FindGlobalEnviromentVariable("MEDIAINFO_DB")
                    ?? (!string.IsNullOrWhiteSpace(configuration.GetConnectionString("MediaInfo")) ? configuration.GetConnectionString("MediaInfo") : throw new ApplicationException("MEDIAINFO_DB NOT SPECIFIED. USE AN ENVIROMENT VAR."));

            logger?.LogInformation($"[SETTINGS]: TVDB_APIKEY - {tvDbOptions.ApiKey}");
            logger?.LogInformation($"[SETTINGS]: MEDIAINFO_DB - {mediaInfoConnString}");

            return services
                .AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(mediaInfoConnString))
                .AddSingleton(tvDbOptions);
        }
    }
}