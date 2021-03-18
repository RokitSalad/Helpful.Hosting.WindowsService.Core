using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WindowsService.Core.Worker
{
    public static class HostBuilderFactory 
    {
        public static IHostBuilder CreateHostBuilder<TWorker>(string[] args) where TWorker : BackgroundService =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TWorker>();
                });

        
    }
}