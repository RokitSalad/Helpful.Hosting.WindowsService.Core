using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Topshelf;

namespace Helpful.Hosting.WindowsService.Core
{
    public class BasicWebService : ServiceControl
    {
        private readonly string[] _urls;
        private static IHost WebServiceHolder { get; set; }

        public BasicWebService(params string[] urls)
        {
            _urls = urls;
        }

        public bool Start(HostControl hostControl)
        {
            WebServiceHolder = CreateHostBuilder(_urls, null).Build();
            WebServiceHolder.StartAsync();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            WebServiceHolder.StopAsync();
            return true;
        }

        private static IHostBuilder CreateHostBuilder(string[] urls, string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(urls);
                });
    }
}