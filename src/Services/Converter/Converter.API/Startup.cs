using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Converter.API.Converter;
using Converter.API.Hosted;
using Converter.API.Models;
using Converter.API.Services;
using Discord.API.Client;
using Discord.API.Client.Client;
using JJ.HostedService;
using JJ.HostedService.Abstraction;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Converter.API {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddLogging(x => x.AddEventLog());

            var handbrakeOptions = Configuration.GetSection("HandbrakeOptions").Get<HandbrakeOptions>();
            services.AddSingleton(handbrakeOptions);

            var stores = Configuration.GetSection("Stores").Get<StoreArea[]>();
            services.AddSingleton(stores);

            var discordOptions = Configuration.GetSection("DiscordOptions").Get<DiscordClientOptions>();
            if (string.IsNullOrWhiteSpace(discordOptions.Address)) throw new ApplicationException("Discord Client address missing.");
            services.AddSingleton(discordOptions);

            var options = Configuration.GetSection("ConverterOptions").Get<ConverterOptions>();
            services.AddSingleton(options);

            services.AddTransient<MediaService>()
                    .AddTransient<IMediaConverter, HandbrakeConverter>()
                    .AddTransient<StoreService>()
                    .AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>()
                    .AddSingleton<DiscordClient>()
                    .AddSingleton<HttpClient>()
                    .AddLogging(builder => builder.AddEventLog())
                    // Hosted Service - Allow this to be DI'd
                    // to allow for enqueueing of background tasks.
                    .AddSingleton(provider => new BackgroundConvertHostedService(
                        new BackgroundTaskQueue(),
                        provider.GetRequiredService<ILogger<BackgroundConvertHostedService>>()
                    ));

            services
                .AddHostedService(provider => provider.GetRequiredService<BackgroundConvertHostedService>());
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