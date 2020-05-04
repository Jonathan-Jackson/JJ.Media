using JJ.Media.MediaInfo.Services;
using JJ.Media.MediaInfo.Services.Interfaces;
using MediaInfo.Domain.DomainLayer.Search;
using MediaInfo.Domain.Helpers.Repository;
using MediaInfo.Domain.Helpers.Repository.Interfaces;
using MediaInfo.Infrastructure.Helpers.Factories;
using MediaInfo.Infrastructure.Helpers.Models;
using MediaInfo.Infrastructure.Remote;
using MediaInfo.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Compilers;
using System;
using TvDbSharper;

namespace MediaInfo.API.ServiceRegister {

    public static class InfrastructureRegister {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
            services
                // Helpers.
                .AddSingleton<ITvDbClient, TvDbClient>()
                .AddScoped<IMemoryCache, MemoryCache>()
                .AddScoped<Compiler>(x => new SqlServerCompiler())
                // Repos
                .AddScoped<IShowRepository, ShowRepository>()
                .AddScoped<IEpisodeRepository, EpisodeRepository>()
                // API
                .AddScoped<IApiSearch, TvDbSearch>()
                .AddScoped<IEpisodeApiSearch, TvDbSearch>()
                .AddScoped<IShowApiSearch, TvDbSearch>();

            // Add Config Dependencies.
            var tvDbOptions = configuration.GetSection("TvDb").Get<TvDbOptions>();
            var mediaInfoConnString = configuration.GetConnectionString("MediaInfo");

            if (string.IsNullOrWhiteSpace(mediaInfoConnString))
                mediaInfoConnString = Environment.GetEnvironmentVariable("MediaInfo_DB", EnvironmentVariableTarget.User) ?? throw new ApplicationException("MediaInfo_DB Database Connection value is missing.");
            if (string.IsNullOrWhiteSpace(tvDbOptions.Token))
                tvDbOptions.Token = Environment.GetEnvironmentVariable("TvDb_Token", EnvironmentVariableTarget.User) ?? throw new ApplicationException("TvDb:Token is missing.");

            return services
                .AddScoped<IDbConnectionFactory>(_ => new SqlConnectionFactory(mediaInfoConnString))
                .AddSingleton(tvDbOptions);
        }
    }
}