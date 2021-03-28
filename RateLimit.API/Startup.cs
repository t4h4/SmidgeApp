using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimit.API
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


            services.AddOptions(); //ratelimit ayarlar� appsettings.json'dan okuyaca��ndan dolay� bunu eklemeliyiz.

            services.AddMemoryCache(); //biriken istekleri ram bellekte tutmak i�in gerekli bir servis. 

            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting")); 

            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));

            //yukar�daki datalar� tutacak memorycash'i belirtmek gerekiyor.
            //uygulama aya�a kalkt�g�nda bir defa y�klensin, bir daha nesne �rne�i al�nmas�n demek i�in AddSingleton diyoruz.
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>(); // ka� request yap�ld�g� datalar�n yeri belirtildi.

            //request yapan�n�n header, ip adresi falan okuyabilmesi i�in eklemek gerekir. 
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //ana rate limit servisi eklemek elzem.
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();



            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RateLimit.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RateLimit.API v1"));
            }

            app.UseClientRateLimiting(); //RateLimit'i kullanmak i�in middleware �zerinden k�s�tlama eklendi.

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
