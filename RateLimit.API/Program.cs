using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreRateLimit;

namespace RateLimit.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webhost = CreateHostBuilder(args).Build();

            // var IpPolicy = webhost.Services.GetRequiredService<IIpPolicyStore>(); //required olan�n� kullan�yoruz ��nk� bu servis program i�in olmazsa olmaz.

            // IpPolicy.SeedAsync().Wait(); //seesasync appsettings i�indeki kurallar� uygulayacak.

            webhost.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
