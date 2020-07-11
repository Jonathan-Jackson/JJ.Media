using Discord.Core.Models.Options;
using Discord.Core.Services;
using JJ.Framework.Helpers;
using JJ.Framework.Helpers.Options;
using JJ.Framework.Repository.Abstraction;
using JJ.Framework.Repository.RabbitMq;
using MediaInfo.API.Client;
using MediaInfo.API.Client.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Discord.Core.ServiceRegister {

    public static class DefaultDependencies {

        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration) {
            services
                // Services
                .AddSingleton<DiscordService>()
                .AddSingleton<IEpisodeAlertService, EpisodeAlertService>()
                .AddSingleton<IMediaInfoClient, MediaInfoClient>()
                // Config
                .AddLogging(configure => configure.AddConsole());

            // Options
            // Broker Options
            var brokerOptions = configuration.GetSection("BrokerOptions").Get<BrokerOptions>();
            brokerOptions.Address = EnviromentHelper.GetSetting("BROKER_ADDRESS", brokerOptions.Address);
            // Discord Options
            var discordOptions = configuration.GetSection("DiscordOptions").Get<DiscordOptions>();
            discordOptions.Token = EnviromentHelper.GetSetting("DISCORD_TOKEN", discordOptions.Token);
            discordOptions.AlertChannelName = EnviromentHelper.GetSetting("DISCORD_ALERTCHANNELNAME", discordOptions.AlertChannelName);
            discordOptions.ViewerDomain = EnviromentHelper.GetSetting("DISCORD_VIEWERDOMAIN", discordOptions.ViewerDomain);
            // MediaInfo Options
            var mediaOptions = configuration.GetSection("MediaInfoOptions").Get<MediaInfoClientOptions>();
            mediaOptions.Address = EnviromentHelper.GetSetting("MEDIAINFO_ADDRESS", mediaOptions.Address);

            return services
                .AddSingleton<HttpClient>()
                .AddSingleton<IMessageBroker, RabbitBroker>(provider => new RabbitBroker(provider.GetRequiredService<BrokerOptions>().Address, provider.GetRequiredService<ILogger<RabbitBroker>>()))
                .AddSingleton(discordOptions)
                .AddSingleton(brokerOptions)
                .AddSingleton(mediaOptions);
        }
    }
}