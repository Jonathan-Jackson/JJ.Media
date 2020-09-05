using Converter.Core.Converters;
using Converter.Core.Helpers.Options;
using Converter.Core.Services;
using JJ.Framework.Helpers;
using JJ.Framework.Helpers.Options;
using JJ.Framework.Repository.Abstraction;
using JJ.Framework.Repository.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Converter.Core.ServiceRegister {

    public static class DependencyModule {

        public static IServiceCollection AddDefaultServices(this IServiceCollection services, IConfiguration config) {
            return services
                // Services
                .AddSingleton<ConverterService>()
                .AddSingleton<IConverter, HandbrakeConverter>()
                .AddSingleton<IFileService, FileService>()
                // Config
                .AddLogging(configure => configure.AddConsole())
                .AddSingleton(BuildHandbrakeOptions(config))
                .AddSingleton(BuildConverterOptions(config))
                .AddSingleton(BuildBrokerOptions(config))
                // Infrastructure
                .AddSingleton<IMessageBroker, RabbitBroker>(provider => {
                    var options = provider.GetRequiredService<BrokerOptions>();
                    return new RabbitBroker(options.Address, options.UserName, options.Password, provider.GetRequiredService<ILogger<RabbitBroker>>());
                });
        }

        #region Option Configuration

        private static ConverterOptions BuildConverterOptions(IConfiguration config) {
            // Get.
            var options = config.GetSection("ConverterOptions").Get<ConverterOptions>();
            options.DownloadStore = EnviromentHelper.GetSetting("CONVERTER_DOWNLOADSTORE", options.DownloadStore);
            options.ProcessingStore = EnviromentHelper.GetSetting("CONVERTER_PROCESSINGSTORE", options.ProcessingStore);
            options.QueueStore = EnviromentHelper.GetSetting("CONVERTER_QUEUESTORE", options.QueueStore);
            options.ProcessedStore = EnviromentHelper.GetSetting("CONVERTER_PROCESSEDSTORE", options.ProcessedStore);

            // Validate.
            if (!Directory.Exists(options.DownloadStore))
                throw new ApplicationException($"Download Store does not exist: {options.DownloadStore}");

            if (!Directory.Exists(options.ProcessingStore))
                throw new ApplicationException($"Processing Store does not exist: {options.ProcessingStore}");

            if (!Directory.Exists(options.QueueStore))
                throw new ApplicationException($"Queue Store does not exist: {options.QueueStore}");

            if (!Directory.Exists(options.ProcessedStore))
                throw new ApplicationException($"Processed Store does not exist: {options.ProcessedStore}");

            return options;
        }

        private static HandbrakeOptions BuildHandbrakeOptions(IConfiguration config) {
            // Get.
            var options = config.GetSection("HandbrakeOptions").Get<HandbrakeOptions>();
            options.CmdPath = EnviromentHelper.GetSetting("HANDBRAKE_CMDPATH", options.CmdPath);
            options.StandardArgs = EnviromentHelper.GetSetting("HANDBRAKE_STANDARDARGS", options.StandardArgs);
            options.SubtitleArgs = EnviromentHelper.GetSetting("HANDBRAKE_SUBTITLEARGS", options.SubtitleArgs);

            // Validate.
            if (!File.Exists(options.CmdPath))
                throw new ApplicationException($"Path to handbrake CLI not found: {options.CmdPath}");

            // Check Args point at a valid json configuration file (if they are pointing at one).
            int pathIndex = options.SubtitleArgs.IndexOf(".json");
            if (pathIndex > -1) {
                string path = StringHelper.GetWordAtIndex(options.SubtitleArgs, pathIndex);
                if (!string.IsNullOrWhiteSpace(path) && !File.Exists(path))
                    throw new ApplicationException($"File used in SubtitleArgs cannot be found: {path} (Full Args: {options.SubtitleArgs}).");
            }

            pathIndex = options.StandardArgs.IndexOf(".json");
            if (pathIndex > -1) {
                string path = StringHelper.GetWordAtIndex(options.StandardArgs, pathIndex);
                if (!string.IsNullOrWhiteSpace(path) && !File.Exists(path))
                    throw new ApplicationException($"File used in StandardArgs cannot be found: {path} (Full Args: {options.StandardArgs}).");
            }

            return options;
        }

        private static BrokerOptions BuildBrokerOptions(IConfiguration config) {
            // Get.
            var options = config.GetSection("BrokerOptions").Get<BrokerOptions>();
            options.Address = EnviromentHelper.GetSetting("BROKER_ADDRESS", options.Address);
            options.UserName = EnviromentHelper.GetSetting("BROKER_USERNAME", options.UserName, allowEmpty: true);
            options.Password = EnviromentHelper.GetSetting("BROKER_PASSWORD", options.Password, allowEmpty: true);

            // Validate.
            if (!RabbitMqHelper.TryConnect(options.Address, options.UserName, options.Password, out Exception ex))
                throw new ApplicationException($"Could not connect to RabbitMq Server on the hostname: {options.Address}, username: {options.UserName}, password: {options.Password} ({ex.Message})");

            return options;
        }

        #endregion Option Configuration
    }
}