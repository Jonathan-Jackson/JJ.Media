using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storage.Core.ServiceRegister;
using Storage.Domain.Services;
using System.Net;
using System.Threading.Tasks;

namespace Storage.Console {

    internal class Program {

        private static async Task Main(string[] args) {
            // Disable SSL ~ Services are ran locally, and for whatever
            // reason Kestral redirects to HTTPS even when specifying HTTP.
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            IServiceCollection services = new ServiceCollection();
            var provider = services.AddDefaultServices(config).BuildServiceProvider();

            await provider.GetRequiredService<StorageService>()
                            .Run();
        }
    }
}