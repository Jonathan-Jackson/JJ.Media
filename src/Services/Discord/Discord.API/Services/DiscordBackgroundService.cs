using Discord.API.Commands;
using Discord.API.Helpers;
using Discord.API.Models.Options;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using JJ.HostedService;
using JJ.HostedService.Abstraction;
using MediaInfo.API.Client.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Storage.API.Client.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API.Services {

    public class DiscordBackgroundService : QueuedHostedService<DiscordClient> {
        private readonly IServiceProvider _provider;
        private readonly DiscordOptions _options;

        private DiscordClient _discord;

        public DiscordBackgroundService(IServiceProvider provider, IBackgroundTaskQueue<DiscordClient> taskQueue, ILogger<DiscordBackgroundService> logger)
             : base(taskQueue, logger, "DiscordBackgroundService") {
            _options = provider.GetRequiredService<DiscordOptions>();
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            _discord ??= await SetupClient();
            await ProcessQueue(_discord, stoppingToken);
        }

        private async Task<DiscordClient> SetupClient() {
            var discord = new DiscordClient(new DiscordConfiguration {
                Token = _options.Token,
                TokenType = TokenType.Bot,
            });

            var dependencies = new Dependency​Collection​Builder()
                    .AddInstance(_options)
                    .AddInstance(_provider.GetRequiredService<MediaInfoClient>())
                    .AddInstance(_provider.GetRequiredService<StorageClient>())
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
            return discord;
        }
    }
}