using MediaViewer.Web.Infrastructure;
using MediaViewer.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Text.Json;

namespace MediaViewer.Web {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();

            var storageOptions = Configuration.GetSection("StorageApiOptions").Get<StorageApiOptions>();

            services.AddSingleton<HttpClient>()
                    .AddTransient<StorageApi>()
                    .AddSingleton(storageOptions)
                    .AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            // MOVE TO ENV SETTING.
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(@"\\htpc\\JJMedia1"),
                RequestPath = "/JJMedia1"
            });
            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(@"\\htpc\\JJMedia2"),
                RequestPath = "/JJMedia2"
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}