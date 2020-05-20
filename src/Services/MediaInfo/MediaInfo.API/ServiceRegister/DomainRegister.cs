using JJ.Media.MediaInfo.Services;
using MediaInfo.Domain.DomainLayer.Search;
using MediaInfo.Domain.Helpers.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaInfo.API.ServiceRegister {

    public static class DomainRegister {

        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration) {
            services
                .AddTransient<EpisodeSearch>()
                .AddTransient<ShowSearch>()
                .AddTransient<ShowStorage>();

            // Add Config Options.
            var storagePaths = configuration.GetSection("Storage").Get<string[]>();

            return services
                .AddSingleton(new MediaInfoConfiguration { StoragePaths = storagePaths });
        }
    }
}