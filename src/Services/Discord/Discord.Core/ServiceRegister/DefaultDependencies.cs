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
using System;
using System.Net.Http;

namespace Discord.Core.ServiceRegister {

    public static class DefaultDependencies {

        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration, ILogger logger = null) {
            services
                // Services
                .AddSingleton<DiscordService>()
                .AddSingleton<IEpisodeAlertService, EpisodeAlertService>()
                .AddSingleton<IMediaInfoClient, MediaInfoClient>()
                // Config
                .AddLogging(configure => configure.AddConsole());

            // Options
            var brokerOptions = GetBrokerOptions(configuration, logger);
            var discordOptions = GetDiscordOptions(configuration, logger);
            var mediaOptions = GetMediaInfoOptions(configuration, logger);

            return services
                .AddSingleton<HttpClient>()
                .AddSingleton<IMessageBroker, RabbitBroker>(provider => {
                    var options = provider.GetRequiredService<BrokerOptions>();
                    return new RabbitBroker(options.Address, options.UserName, options.Password, provider.GetRequiredService<ILogger<RabbitBroker>>());
                })
                .AddSingleton(discordOptions)
                .AddSingleton(brokerOptions)
                .AddSingleton(mediaOptions);
        }

        private static MediaInfoClientOptions GetMediaInfoOptions(IConfiguration configuration, ILogger logger = null) {
            var output = configuration.GetSection("MediaInfoOptions").Get<MediaInfoClientOptions>();
            output.Address = EnviromentHelper.GetSetting("MEDIAINFO_ADDRESS", output.Address);

            // Ensure we can send a message!
            try {
                new MediaInfoClient(new HttpClient(), output)
                    .GetEpisode(100);
            }
            catch (Exception ex) {
                logger?.LogCritical($"Error with setup request to MediaInfo: {output.Address}");
                throw ex;
            }

            logger?.LogInformation($"[SETTINGS]: MEDIAINFO_ADDRESS - {output.Address}");
            return output;
        }

        private static BrokerOptions GetBrokerOptions(IConfiguration configuration, ILogger logger = null) {
            var output = configuration.GetSection("BrokerOptions").Get<BrokerOptions>();
            output.Address = EnviromentHelper.GetSetting("BROKER_ADDRESS", output.Address);
            logger?.LogInformation($"[SETTINGS]: BROKER_ADDRESS (Message Broker Address) - {output.Address}");

            output.UserName = EnviromentHelper.GetSetting("BROKER_USERNAME", output.UserName, allowEmpty: true);
            if (!string.IsNullOrEmpty(output.UserName))
                logger?.LogInformation($"[SETTINGS]: BROKER_USERNAME - '{output.UserName}'");

            output.Password = EnviromentHelper.GetSetting("BROKER_PASSWORD", output.Password, allowEmpty: true);
            if (!string.IsNullOrEmpty(output.Password))
                logger?.LogInformation($"[SETTINGS]: BROKER_PASSWORD - '{output.Password}'");

            if (!RabbitMqHelper.TryConnect(output.Address, output.UserName, output.Password, out Exception ex))
                throw new ApplicationException($"Unable to connect to Broker: {output.Address}, username: {output.UserName}, password: {output.Password} ({ex.Message})");

            return output;
        }

        private static DiscordOptions GetDiscordOptions(IConfiguration configuration, ILogger logger = null) {
            var output = configuration.GetSection("DiscordOptions").Get<DiscordOptions>();

            output.Token = EnviromentHelper.GetSetting("DISCORD_TOKEN", output.Token);
            logger?.LogInformation($"[SETTINGS]: DISCORD_TOKEN (Auth Token) - {output.Token}");

            output.AlertChannelName = EnviromentHelper.GetSetting("DISCORD_ALERTCHANNELNAME", output.AlertChannelName);
            logger?.LogInformation($"[SETTINGS]: DISCORD_ALERTCHANNELNAME (Channel names to show alerts) - {output.AlertChannelName}");

            output.ViewerDomain = EnviromentHelper.GetSetting("DISCORD_VIEWERDOMAIN", output.ViewerDomain);
            logger?.LogInformation($"[SETTINGS]: DISCORD_VIEWERDOMAIN (Web Address to view media) - {output.ViewerDomain}");

            return output;
        }
    }
}