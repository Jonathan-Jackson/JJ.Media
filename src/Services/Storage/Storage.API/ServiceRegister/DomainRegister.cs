using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Domain.DomainLayer.Processor;
using Storage.Domain.DomainLayer.Store;
using Storage.Domain.Helpers.Abstraction;
using Storage.Domain.Helpers.Events;
using Storage.Domain.Helpers.Options;
using Storage.Domain.Plugins;
using System;
using System.Threading.Tasks;

namespace Storage.API.ServiceRegister {

    public static class DomainRegister {

        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration) {
            services
                .AddTransient<EpisodeProcessor>()
                .AddTransient<IEpisodeStore, EpisodePhysicalStore>()
                .AddSingleton(new EventHandler<ProcessedEpisodeEventArgs>(async (s, e) => await Task.CompletedTask));

            // Add Config Options.
            var episodePaths = configuration.GetSection("EpisodeStorageOptions").Get<MediaStorageOptions>();
            var discordOptions = configuration.GetSection("DiscordOptions").Get<DiscordOptions>();

            if (string.IsNullOrWhiteSpace(discordOptions.Token))
                discordOptions.Token = Environment.GetEnvironmentVariable("JJ.NetDiscordToken", EnvironmentVariableTarget.User) ?? throw new ApplicationException("DiscordOptions: Token is missing.");

            services
                .AddSingleton(discordOptions)
                .AddSingleton(episodePaths);

            // Plugins
            return services.AddSingleton<DiscordPlugin>();
        }
    }
}