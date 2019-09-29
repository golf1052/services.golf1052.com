using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace services.golf1052.com
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Build().Run();
        }

        public static IHostBuilder BuildWebHost(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                    })
                    .UseStartup<Startup>()
                    .UseUrls("http://127.0.0.1:8894");
                });
        }
    }
}
