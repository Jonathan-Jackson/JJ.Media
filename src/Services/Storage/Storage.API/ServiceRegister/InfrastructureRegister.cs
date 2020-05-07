using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Domain.Helpers.Repository;
using Storage.Infrastructure.Options;
using Storage.Infrastructure.Remote;
using Storage.Infrastructure.Repositories;

namespace Storage.API.ServiceRegister {

    public static class InfrastructureRegister {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
            services
                .AddTransient<IMediaInfoRepository, MediaInfoApi>()
                .AddTransient<IProcessedRepository, ProcessedRepository>();

            // Add Config Options.
            var mediaInfoOptions = configuration.GetSection("MediaInfoOptions").Get<MediaInfoOptions>();

            return services
                .AddSingleton(mediaInfoOptions);
        }
    }
}