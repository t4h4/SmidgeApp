using FluentValidation.AspNetCore;
using FluentValidationApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
namespace FluentValidationApp
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

            // DI olarak kullanabilmek i�in servis olarak ekledik automapper'�. Art�k IMapper interface'ini herhangi bir class'�n
            // constructor'�nda kulland�g�m zaman, IMapper �zerinden d�n��t�rme i�lemleri ger�ekle�tirebilece�im. IMapper herhangi bir
            // nesneyi Dto'ya ya da tam tersine d�n��t�rmemizi sa�layan haz�r metotlar sunan bir interface. bu interface'i kullanabilmek i�in
            // herhangi bir class'�n constructor'�nda DI olarak ge�ti�im zaman kullanabiliyor olaca��m. Bunu a�a��daki servis sayesinde ger�ekle�tirece�im.
            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration["ConStr"]);
            });

            //IValidator interface'i �zerinden generic olarak customer'�m� al, b�yle bir interface �rne�iyle kar��la��rsan
            // CustomerValidator'dan nesne �rne�i al. 
            /** services.AddSingleton<IValidator<Customer>, CustomerValidator>(); **/
            // yukar�daki tek class validator'� ekliyor. a�a��daki hepsini bulup ekliyor.

            services.AddControllersWithViews().AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true; //model state filtresini invalid et, engelle, ben kendi bildi�imi okuyaca��m, custom hata mesaj� i�in.
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
