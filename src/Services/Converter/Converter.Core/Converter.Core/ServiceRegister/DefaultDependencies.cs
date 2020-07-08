using Converter.Core.Converters;
using Converter.Core.Services;
using JJ.Framework.Helpers;
using JJ.Framework.Helpers.Options;
using JJ.Framework.Repository.Abstraction;
using JJ.Framework.Repository.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Converter.Core.ServiceRegister {

    public static class DefaultDependencies {

        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration) {
            services
                // Services
                .AddSingleton<ConverterService>()
                .AddSingleton<IConverter, HandbrakeConverter>()
                // Config
                .AddLogging(configure => configure.AddConsole());

            // Broker Options
            var brokerOptions = configuration.GetSection("BrokerOptions").Get<BrokerOptions>();
            brokerOptions.Address = EnviromentHelper.FindGlobalEnviromentVariable("BROKER_ADDRESS")
                ?? (!string.IsNullOrWhiteSpace(brokerOptions.Address) ? brokerOptions.Address : throw new ApplicationException("BROKER_ADDRESS NOT SPECIFIED. USE AN ENVIROMENT VAR."));

            return services
                .AddSingleton<IMessageBroker, RabbitBroker>(provider => new RabbitBroker(provider.GetRequiredService<BrokerOptions>().Address, provider.GetRequiredService<ILogger<RabbitBroker>>()));
        }
    }
}