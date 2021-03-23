using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WindowsService.Core.Worker
{
    public static class HostBuilderFactory
    {
        public static IHostBuilder CreateHostBuilder<TWorker>(string[] args, Func<CancellationToken, Task> workerProcess) where TWorker : class, IHostedService =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(provider => workerProcess);
                    services.AddHostedService<TWorker>();
                });

        
    }
}