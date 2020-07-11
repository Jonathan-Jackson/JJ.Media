using Discord.Core;
using Discord.Core.ServiceRegister;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.Tasks;

namespace Discord.Console {

    internal class Program {

        private static async Task Main(string[] args) {
            // Disable SSL ~ Services are ran locally, and for whatever
            // reason Kestral redirects to HTTPS even when specifying HTTP.
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            IServiceCollection services = new ServiceCollection();
            var provider = services.AddDependencies(config).BuildServiceProvider();

            await provider.GetRequiredService<DiscordService>()
                            .Run();
        }
    }
}