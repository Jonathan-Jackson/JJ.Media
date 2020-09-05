using JJ.Framework.Helpers;
using JJ.Media.MediaInfo.API;
using JJ.Media.MediaInfo.Services;
using MediaInfo.Domain.DomainLayer.Search;
using MediaInfo.Domain.Helpers.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MediaInfo.API.ServiceRegister {

    public static class DomainRegister {

        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration, ILogger? logger = null) {
            services
                .AddTransient<EpisodeSearch>()
                .AddTransient<ShowSearch>()
                .AddTransient<ShowStorage>();

            // Add Config Options.
            var storagePaths = Array.Empty<string>();

            if (EnviromentHelper.TryGetGlobalEnviromentVariable("MEDIAINFO_STORAGE", out string envValue)) {
                storagePaths = JsonSerializer.Deserialize<string[]>(envValue);
            }
            else
                storagePaths = configuration.GetSection("Storage").Get<string[]>();

            if (storagePaths.Any(path => !Directory.Exists(path)))
                throw new ApplicationException($"STORAGEPATH DOES NOT EXIST: {string.Join(", ", storagePaths.Where(path => !Directory.Exists(path)))}");

            logger?.LogInformation($"[SETTINGS]: Storage - {string.Join(", ", storagePaths)}");

            return services
                  .AddSingleton(new MediaInfoConfiguration { StoragePaths = storagePaths });
        }
    }
}