using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VueCliMiddleware;

namespace JJ.NET.Web {

    public class Startup {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "App";
            });
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
                    spa.UseVueCli(npmScript: "serve");
                }
            });
        }
    }
}