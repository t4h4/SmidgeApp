using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smidge;
using Smidge.Options;
using Smidge.Cache;

namespace SmidgeApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSmidge(Configuration.GetSection("smidge")); // smidge kutuphanesinin servisini ekledik.

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) //middleware eklenen yer.
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //smidge k�t�phanesi middleware yap�s�. 

            app.UseSmidge(bundle =>
            {
                // EnableCompositeProcessing diyerek debug'da dahi olsa birle�tirme i�lemini ger�ekle�tir. EnableFileWatcher olu�turdugum bundle'da dosyalar� izle, bir de�i�iklik olursa yeniden olu�tur. SetCacheBusterType<AppDomainLifetimeCacheBuster> ile uygulama aya�a kalk�nca cache'i boz ve yeniden olu�tur.
                // browser'a olu�turulan bundle dosyas�n� hi� cache'leme demek i�in de CacheControlOptions giriyoruz. enableEtag: false yap�yoruz. enable tag bir nevi token ve bu token'a g�re browser de�i�iklikleri alg�l�yor. ayn� tag g�nderirse header, o zaman browser de�i�iklik yapm�yor ve cache bellekten ekme�ini yiyor. 304 durum kodu bi �ey de�i�memi� cache den oku demek.
                // biz false yapt�k bu sayede browser cache �al��m�yor ve serverdan bilgileri �ekiyor. cacheControlMaxAge: 0 ise browser cache'yi ne kadar tutabilirin saniye cinsinden de�eri.
                bundle.CreateJs("my-js-bundle", "~/js/").WithEnvironmentOptions(BundleEnvironmentOptions.Create().ForDebug(builder => builder.EnableCompositeProcessing().EnableFileWatcher().SetCacheBusterType<AppDomainLifetimeCacheBuster>().CacheControlOptions(enableEtag: false, cacheControlMaxAge: 0)).Build());

                bundle.CreateCss("my-css-bundle", "~/css/site.css", "~/lib/bootstrap/dist/css/bootstrap.css");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
