using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Topshelf;

namespace Helpful.Hosting.WindowsService.Core
{
    public class BasicWebService : ServiceControl
    {
        private static IHost WebAppHolder { get; set; }

        public bool Start(HostControl hostControl)
        {
            WebAppHolder = CreateHostBuilder(null).Build();
            WebAppHolder.StartAsync();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            WebAppHolder.StopAsync();
            return true;
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:5001/");
                });
    }
}