using Discord.API.Models.Options;
using Discord.API.Services;
using DSharpPlus;
using JJ.HostedService;
using JJ.HostedService.Abstraction;
using MediaInfo.API.Client;
using MediaInfo.API.Client.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Storage.API.Client;
using Storage.API.Client.Client;
using System;
using System.Net.Http;
using System.Text.Json;

namespace Discord.API {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddLogging(config => config.AddConsole().AddEventLog());

            var mediaInfoOptions = Configuration.GetSection("MediaInfoOptions").Get<MediaInfoClientOptions>();
            var storageOptions = Configuration.GetSection("StorageOptions").Get<StorageClientOptions>();
            var discordOptions = Configuration.GetSection("DiscordOptions").Get<DiscordOptions>();

            if (string.IsNullOrWhiteSpace(discordOptions.Token))
                discordOptions.Token = Environment.GetEnvironmentVariable("JJ.NetDiscordToken", EnvironmentVariableTarget.User) ?? throw new ApplicationException("DiscordOptions: Token is missing.");
            if (string.IsNullOrWhiteSpace(discordOptions.ViewerDomain))
                discordOptions.ViewerDomain = Environment.GetEnvironmentVariable("ViewerDomain", EnvironmentVariableTarget.User) ?? throw new ApplicationException("DiscordOptions: Viewer Domain is missing.");

            services
                .AddTransient<EpisodeAlertService>()
                .AddTransient<MediaInfoClient>()
                .AddTransient<StorageClient>()
                .AddTransient<StorageClientOptions>()
                .AddSingleton<IBackgroundTaskQueue<DiscordClient>, BackgroundTaskQueue<DiscordClient>>()
                .AddSingleton<DiscordBackgroundService>()
                .AddSingleton(mediaInfoOptions)
                .AddSingleton(discordOptions)
                .AddSingleton(storageOptions)
                .AddSingleton<HttpClient>()
                .AddSingleton(_ => new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                });

            services.AddHostedService<DiscordBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}