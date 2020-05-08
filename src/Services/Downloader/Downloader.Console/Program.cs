using Downloader.Core.ServiceRegister;
using Downloader.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Downloader.ConsoleUI {

    internal class Program {

        private static async Task Main(string[] args) {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            IServiceCollection services = new ServiceCollection();
            var provider = services.AddDependencies(config).BuildServiceProvider();

            await provider.GetRequiredService<DownloaderService>()
                            .Run();
        }
    }
}