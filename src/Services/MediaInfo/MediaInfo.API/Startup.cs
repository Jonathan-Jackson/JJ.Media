using MediaInfo.API.ServiceRegister;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JJ.Media.MediaInfo.API {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            var logger = CreateTempLogger();

            services.AddControllers();

            services
                .AddDomain(Configuration, logger)
                .AddInfrastructure(Configuration, logger)
                .AddLogging(config => config.AddConsole());
        }

        private ILogger CreateTempLogger() {
            // Log Settings
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConsole();
            });
            return loggerFactory.CreateLogger<Startup>();
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