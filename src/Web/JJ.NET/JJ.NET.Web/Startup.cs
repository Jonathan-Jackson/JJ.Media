using MediaInfo.API.Client;
using MediaInfo.API.Client.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using VueCliMiddleware;

namespace JJ.NET.Web {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "App";
            });

            var mediaInfoOptions = Configuration.GetSection("MediaInfoOptions").Get<MediaInfoClientOptions>();
            services.AddSingleton<MediaInfoClient>()
                    .AddSingleton<HttpClient>()
                    .AddSingleton(mediaInfoOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSpaStaticFiles();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.UseSpa(spa => {
                if (env.IsDevelopment())
                    spa.Options.SourcePath = "App";
                else
                    spa.Options.SourcePath = "dist";

                if (env.IsDevelopment()) {
                    spa.UseVueCli(npmScript: "serve", port: 5000);
                }
            });
        }
    }
}