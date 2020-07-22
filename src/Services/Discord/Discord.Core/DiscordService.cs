using Discord.Core.Commands;
using Discord.Core.Helpers;
using Discord.Core.Models.Options;
using Discord.Core.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using JJ.Framework.Repository.Abstraction;
using MediaInfo.API.Client.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Discord.Core {

    public class DiscordService {
        private readonly ILogger<DiscordService> _log;
        private readonly IMessageBroker _broker;
        private readonly IEpisodeAlertService _episodeAlertService;
        private readonly IMediaInfoClient _mediaClient;
        private readonly DiscordOptions _options;

        private DiscordClient _client;

        private const string DiscordEpisodeAlertQueue = "DiscordEpisodeAlertQueue";

        public DiscordService(ILogger<DiscordService> log, IMessageBroker broker, IEpisodeAlertService episodeAlertService,
                                IMediaInfoClient mediaClient, DiscordOptions options) {
            _log = log;
            _broker = broker;
            _episodeAlertService = episodeAlertService;
            _mediaClient = mediaClient;
            _options = options;
        }

        public async Task Run() {
            _log.LogInformation("Discord Service Ran..");
            await SetupClient();
            SetupBroker();

            while (true) {
                try {
                    _log.LogInformation($"Awaiting broker messages on: {DiscordEpisodeAlertQueue}");
                    await _broker.RecieverAsync<int>(DiscordEpisodeAlertQueue, (episodeId) => _episodeAlertService.Alert(_client, episodeId));
                }
                catch (Exception ex) {
                    _log.LogError(ex, "Fatal error awaiting broker message");
                    await Task.Delay(5000);
                }
            }
        }

        private void SetupBroker() {
            _broker.DeclareQueue(DiscordEpisodeAlertQueue);
            _broker.DeclareExchange("StorageProcessed");
            _broker.BindQueue("StorageProcessed", DiscordEpisodeAlertQueue, "Anime");
            _broker.BindQueue("StorageProcessed", DiscordEpisodeAlertQueue, "Movie");
            _broker.BindQueue("StorageProcessed", DiscordEpisodeAlertQueue, "Show");
        }

        private async Task SetupClient() {
            var discord = new DiscordClient(new DiscordConfiguration {
                Token = _options.Token,
                TokenType = TokenType.Bot,
            });

            var dependencies = new Dependency​Collection​Builder()
                    .AddInstance(_options)
                    .AddInstance(_mediaClient)
                    .Build();

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefix = "!",
                Dependencies = dependencies
            });

            commands.AddCommand<SubscribeCommand>()
                    .AddCommand<ShowCommand>()
                    .AddCommand<WatchCommand>();

            await discord.ConnectAsync();
            // await a heartbeat (allows everything to start loading);
            await Task.Delay(2000);
            await discord.InitializeAsync();

            _log.LogInformation("Discord connection established.");
            _client = discord;
        }
    }
}