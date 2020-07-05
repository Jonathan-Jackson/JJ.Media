using Discord.API.Models.Options;
using Discord.API.Services;
using DSharpPlus;
using JJ.Framework.Helpers;
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
            services.AddLogging(config => config.AddConsole());

            // ENVIROMENTS OVERRIDE APP.CONFIG DEFAULTS.
            // Discord Options.
            var discordOptions = Configuration.GetSection("DiscordOptions").Get<DiscordOptions>();
            discordOptions.Token = EnviromentHelper.FindGlobalEnviromentVariable("JJNETDISCORDTOKEN")
                ?? (!string.IsNullOrWhiteSpace(discordOptions.Token) ? discordOptions.Token : throw new ApplicationException("JJNETDISCORDTOKEN NOT SPECIFIED. USE AN ENVIROMENT VAR."));
            discordOptions.ViewerDomain = EnviromentHelper.FindGlobalEnviromentVariable("VIEWERDOMAIN")
                ?? (!string.IsNullOrWhiteSpace(discordOptions.ViewerDomain) ? discordOptions.ViewerDomain : throw new ApplicationException("VIEWERDOMAIN NOT SPECIFIED. USE AN ENVIROMENT VAR."));
            discordOptions.AlertChannelName = EnviromentHelper.FindGlobalEnviromentVariable("ALERTCHANNELNAME")
                ?? (!string.IsNullOrWhiteSpace(discordOptions.AlertChannelName) ? discordOptions.AlertChannelName : throw new ApplicationException("ALERTCHANNELNAME NOT SPECIFIED. USE AN ENVIROMENT VAR."));

            // MediaInfo Options.
            var mediaInfoOptions = Configuration.GetSection("MediaInfoOptions").Get<MediaInfoClientOptions>();
            mediaInfoOptions.Address = EnviromentHelper.FindGlobalEnviromentVariable("MEDIAINFOADDRESS")
                ?? (!string.IsNullOrWhiteSpace(mediaInfoOptions.Address) ? mediaInfoOptions.Address : throw new ApplicationException("MEDIAINFOADDRESS NOT SPECIFIED. USE AN ENVIROMENT VAR."));

            // Storage Options.
            var storageOptions = Configuration.GetSection("StorageOptions").Get<StorageClientOptions>();
            storageOptions.Address = EnviromentHelper.FindGlobalEnviromentVariable("STORAGEOPTIONSADDRESS")
                ?? (!string.IsNullOrWhiteSpace(storageOptions.Address) ? storageOptions.Address : throw new ApplicationException("STORAGEOPTIONSADDRESS NOT SPECIFIED. USE AN ENVIROMENT VAR."));

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