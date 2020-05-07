using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Domain.DomainLayer.Processor;
using Storage.Domain.DomainLayer.Store;
using Storage.Domain.Helpers.Abstraction;
using Storage.Domain.Helpers.Options;

namespace Storage.API.ServiceRegister {

    public static class DomainRegister {

        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration) {
            services
                .AddTransient<EpisodeProcessor>()
                .AddTransient<IEpisodeStore, EpisodePhysicalStore>();

            // Add Config Options.
            var episodePaths = configuration.GetSection("EpisodeStorageOptions").Get<EpisodeStorageOptions>();

            return services
                .AddSingleton(episodePaths);
        }
    }
}